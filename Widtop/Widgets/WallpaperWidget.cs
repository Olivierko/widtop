using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
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

        private static Wallpaper CreateWallpaper(COM.Rect workArea, COM.DesktopWallpaperPosition position, string path)
        {
            Rectangle rectangle;

            var x = workArea.Left;
            var y = workArea.Top;
            var width = workArea.Right - workArea.Left;
            var height = workArea.Bottom - workArea.Top;

            var image = !string.IsNullOrEmpty(path) && File.Exists(path)
                ? Image.FromFile(path)
                : null;

            // TODO: temporary override
            position = COM.DesktopWallpaperPosition.Fill;

            switch (position)
            {
                case COM.DesktopWallpaperPosition.Center:
                    rectangle = new Rectangle(
                        x + (int)(width * 0.5f) - (int)(image?.Width ?? 0 * 0.5f),
                        y + (int)(height * 0.5f) - (int)(image?.Height ?? 0 * 0.5f),
                        image?.Width ?? 0,
                        image?.Height ?? 0
                    );
                    break;
                case COM.DesktopWallpaperPosition.Fill:
                    rectangle = new Rectangle(
                        x,
                        y,
                        width,
                        height
                    );
                    break;
                case COM.DesktopWallpaperPosition.Tile:
                case COM.DesktopWallpaperPosition.Stretch:
                case COM.DesktopWallpaperPosition.Fit:
                case COM.DesktopWallpaperPosition.Span:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var wallpaper = new Wallpaper(
                rectangle, 
                image
            );

            return wallpaper;
        }

        public override async Task Initialize()
        {
            await Task.Run(() =>
            {
                _wallpapers = new List<Wallpaper>();

                // ReSharper disable once SuspiciousTypeConversion.Global
                var interop = (COM.IDesktopWallpaper)new COM.DesktopWallpaper();

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

                    #region HACK
                    if (monitorIndex == 0)
                    {
                        workArea.Left = 1080;
                        workArea.Top = 294;
                    }
                    else if (monitorIndex == 1)
                    {
                        workArea.Top = 0;
                        workArea.Left = 0;
                    }
                    #endregion

                    var wallpaper = CreateWallpaper(workArea, position, path);

                    _wallpapers.Add(wallpaper);
                }
            });
        }

        public override async Task Render(Graphics graphics)
        {
            await Task.Run(() =>
            {
                foreach (var wallpaper in _wallpapers)
                {
                    lock (wallpaper)
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
            });
        }
    }
}