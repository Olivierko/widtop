using System;
using System.Drawing;
using System.Threading;
using Widtop.Utility;
using Widtop.Widgets;

namespace Widtop
{
    // TODO: retrieve window resolution instead of hard-coding
    internal class Program
    {
        private const int Width = 1920;
        private const int Height = 1200;
        private static Point Anchor => new Point(0, 0);
        private static readonly Bitmap RenderTarget = new Bitmap(Width, Height);

        private static WidgetService _widgetService;

        private static void Main(string[] args)
        {
            _widgetService = new WidgetService();

            lock (RenderTarget)
            {
                _widgetService.Initialize(RenderTarget);
            }

            var updateTimer = new Timer(x => Update(), null, 0, 1000);
            var renderTimer = new Timer(x => Render(), null, 1000, 1000);

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

            lock (RenderTarget)
            {
                using (var graphics = Graphics.FromHdc(deviceContext))
                {
                    graphics.DrawImage(RenderTarget, Anchor);
                }
            }

            U32.ReleaseDC(workerWindow, deviceContext);
        }
    }
}
