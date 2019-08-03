// ReSharper disable InconsistentlySynchronizedField

using System;
using System.Drawing;
using System.Threading;
using Widtop.Utility;
using Widtop.Widgets;

namespace Widtop
{
    // TODO: evaluate if a better strategy for rendering can be implemented (render time avg at 170ms atm)
    // TODO: see over current update loop for widgets, eg: OpenhardwareMonitor seems slow
    internal class Program
    {
        private const int Interval = 1000;

        private static Buffer[] _buffers;
        private static WidgetService _widgetService;

        private static void Main(string[] args)
        {
            var displays = DisplayHandler.GetAll();

            _buffers = new Buffer[displays.Count];

            for (var index = 0; index < displays.Count; index++)
            {
                var display = displays[index];

                var renderTarget = new Bitmap(
                    display.Width, 
                    display.Height
                );

                _buffers[index] = new Buffer(
                    display, 
                    renderTarget
                );
            }

            _widgetService = new WidgetService();
            _widgetService.Initialize(_buffers);

            DesktopHandler.Initialize();
            DesktopHandler.Invalidate();

            var updateTimer = new Timer(x => Update(), null, 0, Interval);
            var renderTimer = new Timer(x => Render(), null, 0, Interval);

            GC.KeepAlive(updateTimer);
            GC.KeepAlive(renderTimer);

            new ManualResetEvent(false).WaitOne();
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

            lock (_buffers)
            {
                using (var graphics = Graphics.FromHdc(deviceContext))
                {
                    foreach (var buffer in _buffers)
                    {
                        graphics.DrawImage(
                            buffer.RenderTarget,
                            new Point(buffer.Display.X, buffer.Display.Y)
                        );
                    }
                }
            }

            Native.ReleaseDC(workerWindow, deviceContext);
        }
    }
}
