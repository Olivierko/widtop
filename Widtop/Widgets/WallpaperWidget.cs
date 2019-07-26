using System.Drawing;
using Microsoft.Win32;

namespace Widtop.Widgets
{
    public class WallpaperWidget : Widget
    {
        private Image _wallpaper;

        public override void Initialize()
        {
            var currentMachine = Registry.CurrentUser;
            var controlPanel = currentMachine.OpenSubKey("Control Panel");
            var desktop = controlPanel?.OpenSubKey("Desktop");

            var wallpaper = desktop?.GetValue("Wallpaper");
            var path = wallpaper as string;

            _wallpaper = !string.IsNullOrEmpty(path)
                ? Image.FromFile(path)
                : null;
        }

        public override void Render(Graphics graphics)
        {
            if (_wallpaper == null)
            {
                return;
            }

            graphics.DrawImage(_wallpaper, new Point(0, 0));
        }
    }
}