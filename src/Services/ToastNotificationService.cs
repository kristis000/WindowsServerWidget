using CommunityToolkit.WinUI.Notifications;

namespace WindowsServerWidget;

internal sealed class ToastNotificationService
{
    private bool _registered;

    public void EnsureRegistered()
    {
        if (_registered)
        {
            return;
        }

        ToastNotificationManagerCompat.OnActivated += _ => { };
        _registered = true;
    }

    public void ShowStateChanged(AppSettings settings, ServerState state)
    {
        EnsureRegistered();

        var stateText = state == ServerState.Online ? "Online" : "Offline";

        new ToastContentBuilder()
            .AddText($"{settings.ServerName} is {stateText}")
            .Show();
    }
}
