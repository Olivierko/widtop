using System;
using System.Drawing;

namespace Widtop.Widgets
{
    public class ClockWidget : Widget
    {
        private static Font Font => new Font("Agency FB", 16);
        private static SolidBrush Brush => new SolidBrush(Color.White);

        public override void Render(Graphics graphics)
        {
            //graphics.FillRectangle(new SolidBrush(Color.Black), 930, 100, 92, 24);
            graphics.DrawString($"{DateTime.Now:HH:mm:ss fff}", Font, Brush, 930, 100);
        }
    }
}