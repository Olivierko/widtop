// ReSharper disable InconsistentlySynchronizedField

using System;
using System.Drawing;
using System.Threading;
using Widtop.Utility;
using Widtop.Widgets;

namespace Widtop
{
    internal class Program
    {
        private const int DisplayIndex = 0;
        private const int Interval = 1000;

        private static Point Anchor => new Point(0, 0);
        private static Bitmap _buffer;

        private static WidgetService _widgetService;

        private static void Main(string[] args)
        {
            if (!Display.TryGetResolution(DisplayIndex, out var width, out var height))
            {
                throw new IndexOutOfRangeException();
            }

            _buffer = new Bitmap(width, height);

            _widgetService = new WidgetService();

            lock (_buffer)
            {
                _widgetService.Initialize(_buffer);
            }

            var updateTimer = new Timer(x => Update(), null, 0, Interval);
            var renderTimer = new Timer(x => Render(), null, 0, Interval);

            GC.KeepAlive(updateTimer);
            GC.KeepAlive(renderTimer);

            new ManualResetEvent(false).WaitOne();
            //DesktopHandler.Invalidate();
        }

        private static void Update()
        {
            _widgetService.Update();
        }

        private static void Render()
        {
            _widgetService.Render();

            if (!DesktopHandler.TryGetWorkerWindow(out var workerWindow))
            {
                return;
            }

            if (!DesktopHandler.TryGetDeviceContext(workerWindow, out var deviceContext))
            {
                return;
            }

            lock (_buffer)
            {
                using (var graphics = Graphics.FromHdc(deviceContext))
                {
                    graphics.DrawImage(_buffer, Anchor);
                }
            }

            U32.ReleaseDC(workerWindow, deviceContext);
        }
    }
}
