// ReSharper disable InconsistentlySynchronizedField

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Widtop.Utility;
using Widtop.Widgets;

namespace Widtop
{
    // TODO: see over current update loop for widgets, eg: Open hardware monitor seems slow
    internal class Program
    {
        private const int Interval = 1000;

        private static Rectangle _area;
        private static IntPtr _workerWindow;
        private static Stopwatch _stopwatch = new Stopwatch();
        private static WidgetService _widgetService;
        private static BufferedGraphicsContext _bufferedGraphicsContext;

        private static void Main(string[] args)
        {
            var displays = DisplayHandler.GetAll();

            var width = 0;
            var height = 0;
            foreach (var display in displays)
            {
                width += display.Width;
                height += display.Height;
            }

            _area = new Rectangle(
                0, 
                0, 
                width, 
                height
            );

            _widgetService = new WidgetService();
            _widgetService.Initialize();

            _bufferedGraphicsContext = new BufferedGraphicsContext();

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
            _stopwatch.Restart();

            if (_workerWindow == IntPtr.Zero && !DesktopHandler.TryGetWorkerWindow(out _workerWindow))
            {
                return;
            }
            
            if (!DesktopHandler.TryGetDeviceContext(_workerWindow, out var deviceContext))
            {
                return;
            }

            using (var bufferedGraphics = _bufferedGraphicsContext.Allocate(deviceContext, _area))
            {
                _widgetService.Render(bufferedGraphics.Graphics);
                bufferedGraphics.Render(deviceContext);
            }
            
            Native.ReleaseDC(_workerWindow, deviceContext);
            Debug.WriteLine($"Render() took: {_stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
