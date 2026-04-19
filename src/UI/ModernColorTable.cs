using System.Drawing;
using System.Windows.Forms;

namespace WindowsServerWidget;

internal sealed class ModernColorTable : ProfessionalColorTable
{
    private readonly AppTheme _theme;

    public ModernColorTable(AppTheme theme)
    {
        _theme = theme;
    }

    public override Color ToolStripDropDownBackground => _theme == AppTheme.Dark
        ? Color.FromArgb(39, 39, 42)
        : Color.FromArgb(249, 249, 249);

    public override Color ImageMarginGradientBegin => ToolStripDropDownBackground;

    public override Color ImageMarginGradientMiddle => ToolStripDropDownBackground;

    public override Color ImageMarginGradientEnd => ToolStripDropDownBackground;

    public override Color MenuBorder => _theme == AppTheme.Dark
        ? Color.FromArgb(68, 68, 74)
        : Color.FromArgb(221, 221, 221);

    public override Color MenuItemBorder => _theme == AppTheme.Dark
        ? Color.FromArgb(78, 89, 106)
        : Color.FromArgb(204, 228, 247);

    public override Color MenuItemSelected => _theme == AppTheme.Dark
        ? Color.FromArgb(53, 62, 74)
        : Color.FromArgb(229, 241, 251);

    public override Color MenuItemSelectedGradientBegin => MenuItemSelected;

    public override Color MenuItemSelectedGradientEnd => MenuItemSelected;

    public override Color SeparatorDark => _theme == AppTheme.Dark
        ? Color.FromArgb(68, 68, 74)
        : Color.FromArgb(226, 226, 226);

    public override Color SeparatorLight => SeparatorDark;
}
