using System.Drawing;
using System.Windows.Forms;

namespace WindowsServerWidget;

internal sealed class SettingsForm : Form
{
    private readonly TextBox _nameTextBox;
    private readonly TextBox _hostTextBox;
    private readonly TextBox _portTextBox;
    private readonly TextBox _timeoutTextBox;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;
    private readonly Label[] _labels;

    public event EventHandler<AppSettings>? SettingsSaved;

    private static class UiLayout
    {
        public const int WindowWidth = 420;
        public const int WindowHeight = 340;

        public const int MarginLeft = 24;
        public const int MarginRight = 24;
        public const int TopPadding = 28;
        public const int BottomPadding = 24;

        public const int LabelHeight = 22;
        public const int InputHeight = 28;
        public const int LabelToInputGap = 6;
        public const int SectionGap = 16;
        public const int RowGap = 20;
        public const int ColumnGap = 12;
        public const int InputToButtonGap = 40;
        public const int ButtonHeight = 34;

        public static int ContentWidth => WindowWidth - MarginLeft - MarginRight;
        public static int HalfWidth => (ContentWidth - ColumnGap) / 2;
    }

    public SettingsForm(AppSettings settings)
    {
        Text = "Server Status Settings";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ShowInTaskbar = true;
        ShowIcon = true;
        Font = ModernUi.Fonts.Ui;
        ClientSize = new Size(UiLayout.WindowWidth, UiLayout.WindowHeight);
        AutoScaleMode = AutoScaleMode.Dpi;
        Icon = ResolveWindowIcon();

        var y = UiLayout.TopPadding;

        var nameLabel = CreateLabel("Server name", UiLayout.MarginLeft, y, UiLayout.ContentWidth);
        y += UiLayout.LabelHeight + UiLayout.LabelToInputGap;
        _nameTextBox = CreateTextBox(settings.ServerName, UiLayout.MarginLeft, y, UiLayout.ContentWidth);
        y += UiLayout.InputHeight + UiLayout.SectionGap;

        var hostLabel = CreateLabel("Host / IP", UiLayout.MarginLeft, y, UiLayout.ContentWidth);
        y += UiLayout.LabelHeight + UiLayout.LabelToInputGap;
        _hostTextBox = CreateTextBox(settings.Host, UiLayout.MarginLeft, y, UiLayout.ContentWidth);
        y += UiLayout.InputHeight + UiLayout.RowGap;

        var leftColumn = UiLayout.MarginLeft;
        var rightColumn = UiLayout.MarginLeft + UiLayout.HalfWidth + UiLayout.ColumnGap;

        var portLabel = CreateLabel("TCP port", leftColumn, y, UiLayout.HalfWidth);
        var timeoutLabel = CreateLabel("Timeout (ms)", rightColumn, y, UiLayout.HalfWidth);
        y += UiLayout.LabelHeight + UiLayout.LabelToInputGap;

        _portTextBox = CreateTextBox(settings.Port.ToString(), leftColumn, y, UiLayout.HalfWidth);
        _timeoutTextBox = CreateTextBox(settings.ConnectTimeoutMs.ToString(), rightColumn, y, UiLayout.HalfWidth);
        y += UiLayout.InputHeight + UiLayout.InputToButtonGap;

        _cancelButton = CreateButton("Cancel", false, leftColumn, y, UiLayout.HalfWidth);
        _saveButton = CreateButton("Save", true, rightColumn, y, UiLayout.HalfWidth);

        _saveButton.Click += SaveButton_Click;
        _cancelButton.Click += (_, _) => Close();

        _labels = new[] { nameLabel, hostLabel, portLabel, timeoutLabel };

        Controls.AddRange(new Control[]
        {
            nameLabel,
            _nameTextBox,
            hostLabel,
            _hostTextBox,
            portLabel,
            _portTextBox,
            timeoutLabel,
            _timeoutTextBox,
            _cancelButton,
            _saveButton
        });

        AcceptButton = _saveButton;
        CancelButton = _cancelButton;

        ApplyTheme(ThemeService.CurrentTheme);
        ThemeService.ThemeChanged += ThemeService_ThemeChanged;
    }

    public AppSettings? UpdatedSettings { get; private set; }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ApplyTheme(ThemeService.CurrentTheme);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        ThemeService.ThemeChanged -= ThemeService_ThemeChanged;
        base.OnFormClosed(e);
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        var serverName = _nameTextBox.Text.Trim();
        var host = _hostTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(serverName))
        {
            MessageBox.Show(this, "Enter a server name.", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(host))
        {
            MessageBox.Show(this, "Enter a host name or IP address.", "Invalid host", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!TryParseInt(_portTextBox.Text, 1, 65535, out var port))
        {
            MessageBox.Show(this, "Enter a TCP port between 1 and 65535.", "Invalid TCP port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!TryParseInt(_timeoutTextBox.Text, 250, 30000, out var timeoutMs))
        {
            MessageBox.Show(this, "Enter a timeout between 250 and 30000 milliseconds.", "Invalid timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UpdatedSettings = new AppSettings
        {
            ServerName = serverName,
            Host = host,
            Port = port,
            ConnectTimeoutMs = timeoutMs,
            PollIntervalSeconds = 5
        };

        SettingsSaved?.Invoke(this, UpdatedSettings);
        Close();
    }

    private void ThemeService_ThemeChanged(object? sender, AppTheme theme)
    {
        if (!IsHandleCreated || IsDisposed)
        {
            return;
        }

        BeginInvoke(new Action(() => ApplyTheme(theme)));
    }

    private void ApplyTheme(AppTheme theme)
    {
        var palette = ModernUi.GetPalette(theme);
        BackColor = palette.WindowBackground;
        ForeColor = palette.PrimaryText;

        foreach (var label in _labels)
        {
            label.ForeColor = palette.SecondaryText;
            label.BackColor = palette.WindowBackground;
        }

        ApplyInputTheme(_nameTextBox, palette);
        ApplyInputTheme(_hostTextBox, palette);
        ApplyInputTheme(_portTextBox, palette);
        ApplyInputTheme(_timeoutTextBox, palette);

        ApplyButtonTheme(_saveButton, palette.PrimaryButtonBackground, palette.PrimaryButtonForeground, palette.PrimaryButtonBackground);
        ApplyButtonTheme(_cancelButton, palette.SecondaryButtonBackground, palette.SecondaryButtonForeground, palette.SurfaceBorder);

        var darkModeValue = theme == AppTheme.Dark ? 1 : 0;
        if (IsHandleCreated)
        {
            NativeMethods.DwmSetWindowAttribute(Handle, NativeMethods.DwmwaUseImmersiveDarkMode, ref darkModeValue, sizeof(int));
        }
    }

    private static void ApplyInputTheme(Control control, ThemePalette palette)
    {
        control.BackColor = palette.InputBackground;
        control.ForeColor = palette.InputForeground;
    }

    private static void ApplyButtonTheme(Button button, Color backColor, Color foreColor, Color borderColor)
    {
        button.BackColor = backColor;
        button.ForeColor = foreColor;
        button.FlatAppearance.BorderColor = borderColor;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.1f);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.05f);
    }

    private static Label CreateLabel(string text, int left, int top, int width)
    {
        return new Label
        {
            Left = left,
            Top = top,
            Width = width,
            Height = UiLayout.LabelHeight,
            Text = text
        };
    }

    private static TextBox CreateTextBox(string value, int left, int top, int width)
    {
        return new TextBox
        {
            Left = left,
            Top = top,
            Width = width,
            Height = UiLayout.InputHeight,
            Text = value,
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    private static Button CreateButton(string text, bool primary, int left, int top, int width)
    {
        var button = new Button
        {
            Left = left,
            Top = top,
            Width = width,
            Height = UiLayout.ButtonHeight,
            Text = text,
            FlatStyle = FlatStyle.Flat,
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 1;
        return button;
    }

    private static bool TryParseInt(string value, int min, int max, out int parsed)
    {
        if (int.TryParse(value.Trim(), out parsed) && parsed >= min && parsed <= max)
        {
            return true;
        }

        parsed = default;
        return false;
    }

    private static Icon? ResolveWindowIcon()
    {
        try
        {
            var executablePath = Environment.ProcessPath;
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                return null;
            }

            using var extractedIcon = Icon.ExtractAssociatedIcon(executablePath);
            return extractedIcon?.Clone() as Icon;
        }
        catch
        {
            return null;
        }
    }
}
