using Microsoft.Win32;

namespace WindowsServerWidget;

internal sealed class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "ServerStatus";
    private const string LegacyValueName = "WindowsServerWidget";

    public void Apply(bool enabled)
    {
        if (enabled)
        {
            var executablePath = Environment.ProcessPath;
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                return;
            }

            EnsureRunKeyEntry(executablePath);
            return;
        }

        RemoveRunKeyEntry();
    }

    private static void EnsureRunKeyEntry(string executablePath)
    {
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

    private static void RemoveRunKeyEntry()
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath);
        if (key is null)
        {
            return;
        }

        key.DeleteValue(ValueName, false);
        key.DeleteValue(LegacyValueName, false);
    }
}
