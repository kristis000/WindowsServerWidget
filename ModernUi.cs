using System.Drawing;
using System.Windows.Forms;

namespace WindowsServerWidget;

internal static class ModernUi
{
    public static class Fonts
    {
        private static readonly FontFamily UiFamily = ResolveFamily("Segoe UI Variable Text", "Segoe UI");
        private static readonly FontFamily TitleFamily = ResolveFamily("Segoe UI Variable Display", "Segoe UI");

        public static Font Ui => new(UiFamily, 9f, FontStyle.Regular);

        public static Font Title => new(TitleFamily, 12f, FontStyle.Bold);

        private static FontFamily ResolveFamily(string preferredName, string fallbackName)
        {
            return FontFamily.Families.FirstOrDefault(f => string.Equals(f.Name, preferredName, StringComparison.OrdinalIgnoreCase))
                ?? FontFamily.Families.First(f => string.Equals(f.Name, fallbackName, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static ThemePalette GetPalette(AppTheme theme)
    {
        return theme == AppTheme.Dark
            ? new ThemePalette(
                Color.FromArgb(32, 32, 32),
                Color.FromArgb(241, 241, 241),
                Color.FromArgb(163, 163, 172),
                Color.FromArgb(47, 47, 51),
                Color.FromArgb(241, 241, 241),
                Color.FromArgb(60, 60, 67),
                Color.FromArgb(15, 98, 254),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(47, 47, 51),
                Color.FromArgb(241, 241, 241),
                Color.FromArgb(68, 68, 74))
            : new ThemePalette(
                Color.FromArgb(243, 243, 243),
                Color.FromArgb(24, 24, 27),
                Color.FromArgb(60, 60, 67),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(24, 24, 27),
                Color.FromArgb(217, 217, 223),
                Color.FromArgb(0, 95, 184),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(24, 24, 27),
                Color.FromArgb(207, 207, 214));
    }
}

internal enum AppTheme
{
    Light,
    Dark
}

internal sealed record ThemePalette(
    Color WindowBackground,
    Color PrimaryText,
    Color SecondaryText,
    Color InputBackground,
    Color InputForeground,
    Color InputBorder,
    Color PrimaryButtonBackground,
    Color PrimaryButtonForeground,
    Color SecondaryButtonBackground,
    Color SecondaryButtonForeground,
    Color SurfaceBorder);
