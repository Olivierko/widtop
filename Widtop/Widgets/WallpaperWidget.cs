using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Widtop.Utility;

namespace Widtop.Widgets
{
    // TODO: construct an image based on the position setting
    public class WallpaperWidget : Widget
    {
        private class Wallpaper
        {
            public Rectangle Area { get; }
            public Image Image { get; }

            public Wallpaper(Rectangle area, Image image)
            {
                Area = area;
                Image = image;
            }
        }

        private SolidBrush _backgroundBrush;
        private List<Wallpaper> _wallpapers;

        private static Color ToColor(uint colorHex)
        {
            var r = (byte)(colorHex >> 0);
            var g = (byte)(colorHex >> 8);
            var b = (byte)(colorHex >> 16);

            return Color.FromArgb(r, g, b);
        }

        public override void Initialize()
        {
            _wallpapers = new List<Wallpaper>();

            // ReSharper disable once SuspiciousTypeConversion.Global
            var interop = (COM.IDesktopWallpaper) new COM.DesktopWallpaper();

            var colorHex = interop.GetBackgroundColor();
            var backgroundColor = ToColor(colorHex);
            _backgroundBrush = new SolidBrush(backgroundColor);

            var monitorCount = interop.GetMonitorDevicePathCount();

            for (uint monitorIndex = 0; monitorIndex < monitorCount; monitorIndex++)
            {
                var devicePath = interop.GetMonitorDevicePathAt(monitorIndex);

                var path = interop.GetWallpaper(devicePath);

                var workArea = interop.GetMonitorRECT(devicePath);

                var position = interop.GetPosition();

                var rectangle = new Rectangle(
                    workArea.Left, 
                    workArea.Top, 
                    workArea.Right - workArea.Left, 
                    workArea.Bottom - workArea.Top
                );

                var image = !string.IsNullOrEmpty(path) && File.Exists(path) 
                    ? Image.FromFile(path) 
                    : null;

                var wallpaper = new Wallpaper(rectangle, image);

                _wallpapers.Add(wallpaper);
            }
        }

        public override void Render(Graphics graphics)
        {
            foreach (var wallpaper in _wallpapers)
            {
                if (wallpaper.Image != null)
                {
                    graphics.DrawImageUnscaled(wallpaper.Image, new Point(wallpaper.Area.X, wallpaper.Area.Y));
                }
                else
                {
                    graphics.FillRectangle(_backgroundBrush, wallpaper.Area);
                }
            }
        }
    }
}