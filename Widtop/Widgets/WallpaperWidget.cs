using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Widtop.Utility;

namespace Widtop.Widgets
{
    public class WallpaperWidget : Widget
    {
        private SolidBrush _backgroundBrush;
        private Dictionary<uint, Image> _wallpapers;

        private static Color ToColor(uint colorHex)
        {
            var r = (byte)(colorHex >> 0);
            var g = (byte)(colorHex >> 8);
            var b = (byte)(colorHex >> 16);

            return Color.FromArgb(r, g, b);
        }

        public override void Initialize()
        {
            _wallpapers = new Dictionary<uint, Image>();

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

                // TODO: construct an image based on the position setting
                //var position = interop.GetPosition();

                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (!File.Exists(path))
                {
                    continue;
                }

                _wallpapers[monitorIndex] = Image.FromFile(path);
            }
        }

        public override void Render(Buffer buffer, Graphics graphics)
        {
            if (_wallpapers.TryGetValue(buffer.Display.Index, out var wallpaper))
            {
                graphics.DrawImage(wallpaper, new Point(0, 0));
            }
            else
            {
                var area = new Rectangle(
                    0, 
                    0, 
                    buffer.Display.Width, 
                    buffer.Display.Height
                );

                graphics.FillRectangle(_backgroundBrush, area);
            }
        }
    }
}