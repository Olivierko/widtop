using System;
using System.Drawing;

namespace Widtop.Widgets
{
    public class ClockWidget : Widget
    {
        private static Font Font => new Font("Agency FB", 18);
        private static SolidBrush Brush => new SolidBrush(Color.White);

        public override void Render(Graphics graphics)
        {
            graphics.DrawString($"{DateTime.Now:HH:mm:ss}", Font, Brush, 880, 350);
        }
    }
}