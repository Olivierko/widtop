using System;
using System.Drawing;
using Widtop.Utility;

namespace Widtop
{
    public class Buffer
    {
        public DisplayHandler.Display Display { get; }
        public Bitmap RenderTarget { get; }

        public Buffer(DisplayHandler.Display display, Bitmap renderTarget)
        {
            Display = display;
            RenderTarget = renderTarget;
        }

        public bool Matches(Point point, out Point localPoint)
        {
            var nativePoint = new Native.Point
            {
                X = point.X,
                Y = point.Y
            };

            var hMonitor = Native.MonitorFromPoint(
                nativePoint,
                Native.MonitorOptions.MONITOR_DEFAULTTONULL
            );

            if (hMonitor == IntPtr.Zero)
            {
                localPoint = Point.Empty;
                return false;
            }

            var matches = hMonitor == Display.Handle;

            if (!matches)
            {
                localPoint = Point.Empty;
                return false;
            }

            var localX = nativePoint.X - Display.X;
            var localY = nativePoint.Y - Display.Y;

            localPoint = new Point(
                localX,
                localY
            );

            return true;
        }

        public bool Matches(Rectangle area, out Rectangle localArea)
        {
            var point = new Point(
                area.X, 
                area.Y
            );

            var matches = Matches(point, out var localPoint);

            if (!matches)
            {
                localArea = Rectangle.Empty;
                return false;
            }

            localArea = new Rectangle(
                localPoint.X, 
                localPoint.Y, 
                area.Width, 
                area.Height
            );

            return true;
        }
    }
}