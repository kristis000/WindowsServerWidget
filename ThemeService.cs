using Microsoft.Win32;

namespace WindowsServerWidget;

internal static class ThemeService
{
    private const string PersonalizeKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string AppsUseLightThemeValueName = "AppsUseLightTheme";

    static ThemeService()
    {
        CurrentTheme = ReadTheme();
        SystemEvents.UserPreferenceChanged += (_, e) =>
        {
            if (e.Category is UserPreferenceCategory.General or UserPreferenceCategory.Color)
            {
                RefreshTheme();
            }
        };
    }

    public static event EventHandler<AppTheme>? ThemeChanged;

    public static AppTheme CurrentTheme { get; private set; }

    public static void RefreshTheme()
    {
        var theme = ReadTheme();
        if (theme == CurrentTheme)
        {
            return;
        }

        CurrentTheme = theme;
        ThemeChanged?.Invoke(null, theme);
    }

    private static AppTheme ReadTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(PersonalizeKeyPath);
            var value = key?.GetValue(AppsUseLightThemeValueName);
            return value is int intValue && intValue == 0 ? AppTheme.Dark : AppTheme.Light;
        }
        catch
        {
            return AppTheme.Light;
        }
    }
}
