using System.Drawing;
using System.Windows.Forms;

namespace WindowsServerWidget;

internal sealed class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ContextMenuStrip _menu;
    private readonly SettingsStore _settingsStore;
    private readonly TcpServerProbe _probe;
    private readonly ToastNotificationService _toastNotifications;
    private readonly Control _uiInvoker;
    private readonly StartupRegistration _startupRegistration;
    private AppSettings _settings;
    private SettingsForm? _settingsForm;
    private CancellationTokenSource? _monitorCts;
    private ServerState _currentState = ServerState.Unknown;
    private Icon? _currentIcon;
    private int _checkInProgress;

    public TrayApplicationContext()
    {
        _settingsStore = new SettingsStore();
        _probe = new TcpServerProbe();
        _toastNotifications = new ToastNotificationService();
        _startupRegistration = new StartupRegistration();
        _settings = _settingsStore.Load();
        _uiInvoker = new Control();
        _uiInvoker.CreateControl();

        _menu = new RoundedContextMenuStrip();
        _menu.Font = ModernUi.Fonts.Ui;
        _menu.ShowImageMargin = false;
        _menu.Items.Add("Settings", null, (_, _) => ShowSettings());
        _menu.Items.Add("Check now", null, async (_, _) => await CheckNowAsync());
        _menu.Items.Add(new ToolStripSeparator());
        _menu.Items.Add("Exit", null, (_, _) => ExitThread());
        ApplyTheme(ThemeService.CurrentTheme);
        ThemeService.ThemeChanged += ThemeService_ThemeChanged;

        _notifyIcon = new NotifyIcon
        {
            ContextMenuStrip = _menu,
            Text = "Server Status",
            Visible = true
        };
        _notifyIcon.DoubleClick += (_, _) => ShowSettings();

        UpdateState(ServerState.Unknown, "Waiting for first check.");
        _toastNotifications.EnsureRegistered();
        _startupRegistration.EnsureRegistered();
        StartMonitoring();
    }

    protected override void ExitThreadCore()
    {
        _monitorCts?.Cancel();
        _settingsForm?.Close();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _menu.Dispose();
        _currentIcon?.Dispose();
        _uiInvoker.Dispose();
        _monitorCts?.Dispose();
        ThemeService.ThemeChanged -= ThemeService_ThemeChanged;
        base.ExitThreadCore();
    }

    private void StartMonitoring()
    {
        _monitorCts?.Cancel();
        _monitorCts?.Dispose();
        _monitorCts = new CancellationTokenSource();
        _ = MonitorLoopAsync(_monitorCts.Token);
    }

    private async Task MonitorLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_settings.PollIntervalSeconds));
        await CheckNowAsync();

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await CheckNowAsync();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task CheckNowAsync()
    {
        if (Interlocked.Exchange(ref _checkInProgress, 1) == 1)
        {
            return;
        }

        try
        {
            var result = await _probe.ProbeAsync(_settings, _monitorCts?.Token ?? CancellationToken.None);
            var previousState = _currentState;
            await InvokeOnUiAsync(() =>
            {
                UpdateState(result.State, result.Detail);

                if (previousState != ServerState.Unknown &&
                    previousState != result.State)
                {
                    _toastNotifications.ShowStateChanged(_settings, result.State);
                }
            });
        }
        finally
        {
            Interlocked.Exchange(ref _checkInProgress, 0);
        }
    }

    private void UpdateState(ServerState state, string detail)
    {
        _currentState = state;
        _currentIcon?.Dispose();
        _currentIcon = TrayIconFactory.Create(state);
        _notifyIcon.Icon = _currentIcon;

        var label = state switch
        {
            ServerState.Online => "Online",
            ServerState.Offline => "Offline",
            _ => "Unknown"
        };

        _notifyIcon.Text = FormatTrayText(label);
        _notifyIcon.BalloonTipTitle = "Windows Server Widget";
        _notifyIcon.BalloonTipText = detail;
    }

    private void ThemeService_ThemeChanged(object? sender, AppTheme theme)
    {
        if (_uiInvoker.IsDisposed)
        {
            return;
        }

        _uiInvoker.BeginInvoke(new Action(() => ApplyTheme(theme)));
    }

    private void ApplyTheme(AppTheme theme)
    {
        var palette = ModernUi.GetPalette(theme);
        _menu.Renderer = new RoundedMenuRenderer(theme);
        _menu.BackColor = palette.WindowBackground;
        _menu.ForeColor = palette.PrimaryText;

        foreach (ToolStripItem item in _menu.Items)
        {
            item.BackColor = palette.WindowBackground;
            item.ForeColor = palette.PrimaryText;
        }
    }

    private Task InvokeOnUiAsync(Action action)
    {
        if (!_uiInvoker.InvokeRequired)
        {
            action();
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _uiInvoker.BeginInvoke(new Action(() =>
        {
            try
            {
                action();
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }));

        return tcs.Task;
    }

    private void ShowSettings()
    {
        if (_settingsForm is not null && !_settingsForm.IsDisposed)
        {
            if (_settingsForm.WindowState == FormWindowState.Minimized)
            {
                _settingsForm.WindowState = FormWindowState.Normal;
            }

            _settingsForm.BringToFront();
            _settingsForm.Activate();
            return;
        }

        _settingsForm = new SettingsForm(_settings);
        _settingsForm.SettingsSaved += (_, updatedSettings) =>
        {
            _settings = updatedSettings;
            _settingsStore.Save(_settings);
            StartMonitoring();
        };
        _settingsForm.FormClosed += (_, _) => _settingsForm = null;
        _settingsForm.Show();
        _settingsForm.BringToFront();
        _settingsForm.Activate();
    }

    private string FormatTrayText(string stateLabel)
    {
        var text = $"{_settings.ServerName}: {stateLabel}";
        return text.Length <= 63 ? text : text[..63];
    }
}
