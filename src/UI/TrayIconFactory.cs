using System.Drawing;
using System.Drawing.Drawing2D;

namespace WindowsServerWidget;

internal static class TrayIconFactory
{
    public static Icon Create(ServerState state)
    {
        var color = state switch
        {
            ServerState.Online => Color.FromArgb(36, 168, 84),
            ServerState.Offline => Color.FromArgb(207, 66, 69),
            _ => Color.FromArgb(126, 134, 142)
        };

        using var bitmap = new Bitmap(16, 16);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var borderPen = new Pen(Color.FromArgb(50, 50, 50));
        using var fillBrush = new SolidBrush(color);
        graphics.FillEllipse(fillBrush, 1, 1, 13, 13);
        graphics.DrawEllipse(borderPen, 1, 1, 13, 13);

        var handle = bitmap.GetHicon();
        try
        {
            return Icon.FromHandle(handle).Clone() as Icon ?? SystemIcons.Application;
        }
        finally
        {
            NativeMethods.DestroyIcon(handle);
        }
    }
}
