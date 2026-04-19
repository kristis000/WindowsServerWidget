using Microsoft.Win32;

namespace WindowsServerWidget;

internal sealed class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "ServerStatus";
    private const string LegacyValueName = "WindowsServerWidget";

    public void EnsureRegistered()
    {
        var executablePath = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(executablePath))
        {
            return;
        }

        using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath);
        if (key is null)
        {
            return;
        }

        var expectedValue = $"\"{executablePath}\"";
        var currentValue = key.GetValue(ValueName) as string;
        if (!string.Equals(currentValue, expectedValue, StringComparison.Ordinal))
        {
            key.SetValue(ValueName, expectedValue);
        }

        if (key.GetValue(LegacyValueName) is not null)
        {
            key.DeleteValue(LegacyValueName, false);
        }
    }
}
