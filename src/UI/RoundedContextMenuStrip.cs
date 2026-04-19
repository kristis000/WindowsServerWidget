using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsServerWidget;

internal sealed class RoundedContextMenuStrip : ContextMenuStrip
{
    private const int CornerRadius = 12;

    public RoundedContextMenuStrip()
    {
        Padding = new Padding(6);
    }

    protected override void OnOpening(System.ComponentModel.CancelEventArgs e)
    {
        base.OnOpening(e);
        ApplyRoundedRegion();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        ApplyRoundedRegion();
    }

    private void ApplyRoundedRegion()
    {
        if (Width <= 1 || Height <= 1)
        {
            return;
        }

        using var path = new GraphicsPath();
        var rect = new Rectangle(0, 0, Width, Height);
        var diameter = CornerRadius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        Region?.Dispose();
        Region = new Region(path);
    }
}
