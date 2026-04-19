using System.Windows.Forms;

namespace WindowsServerWidget;

internal sealed class RoundedMenuRenderer : ToolStripProfessionalRenderer
{
    public RoundedMenuRenderer(AppTheme theme)
        : base(new ModernColorTable(theme))
    {
        RoundedEdges = true;
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        // Suppress the default rectangular border so the rounded region stays consistent.
    }
}
