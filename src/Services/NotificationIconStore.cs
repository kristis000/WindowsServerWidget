using System.Drawing;
using System.Drawing.Imaging;

namespace WindowsServerWidget;

internal sealed class NotificationIconStore
{
    private readonly string _rootPath;

    public NotificationIconStore()
    {
        _rootPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ServerStatus",
            "icons");
        Directory.CreateDirectory(_rootPath);
    }

    public string EnsurePngIcon(ServerState state, int size = 64)
    {
        var fileName = state switch
        {
            ServerState.Online => $"online-{size}.png",
            ServerState.Offline => $"offline-{size}.png",
            _ => $"unknown-{size}.png"
        };

        var path = Path.Combine(_rootPath, fileName);
        if (File.Exists(path))
        {
            return path;
        }

        using var bitmap = new Bitmap(size, size);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var fillColor = state switch
        {
            ServerState.Online => Color.FromArgb(36, 168, 84),
            ServerState.Offline => Color.FromArgb(207, 66, 69),
            _ => Color.FromArgb(126, 134, 142)
        };

        var inset = Math.Max(1, size / 8);
        var diameter = size - (inset * 2);
        var shadowOffset = Math.Max(1, size / 32);
        var borderWidth = Math.Max(1f, size / 32f);

        using var shadowBrush = new SolidBrush(Color.FromArgb(45, 0, 0, 0));
        using var fillBrush = new SolidBrush(fillColor);
        using var borderPen = new Pen(Color.FromArgb(220, 255, 255, 255), borderWidth);
        graphics.FillEllipse(shadowBrush, inset + shadowOffset, inset + shadowOffset, diameter, diameter);
        graphics.FillEllipse(fillBrush, inset, inset, diameter, diameter);
        graphics.DrawEllipse(borderPen, inset, inset, diameter, diameter);

        bitmap.Save(path, ImageFormat.Png);
        return path;
    }
}
