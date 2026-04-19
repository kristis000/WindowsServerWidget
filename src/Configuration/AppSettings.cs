namespace WindowsServerWidget;

internal sealed class AppSettings
{
    public bool StartOnWindowsStartup { get; set; } = true;

    public string ServerName { get; set; } = "Ubuntu Server";

    public string Host { get; set; } = "ubuntu.local";

    public int Port { get; set; } = 22;

    public int PollIntervalSeconds { get; set; } = 5;

    public int ConnectTimeoutMs { get; set; } = 1500;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ServerName) &&
        !string.IsNullOrWhiteSpace(Host) &&
        Port is > 0 and <= 65535 &&
        PollIntervalSeconds > 0 &&
        ConnectTimeoutMs > 0;
}
