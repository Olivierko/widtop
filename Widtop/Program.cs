using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using Widtop.Utility;
using Widtop.Widgets;

namespace Widtop
{
    // TODO: figure out how to invalidate a device context / window instead of clearing on invalidate(), RedrawWindow in user32 didn't work
    internal class Program
    {
        private const int UpdateInterval = 100;
        private const int RenderInterval = 1000;

        private static readonly Stopwatch UpdateStopWatch = new Stopwatch();
        private static readonly Stopwatch RenderStopWatch = new Stopwatch();

        private static Rectangle _area;
        private static IntPtr _workerWindow;
        private static WidgetService _widgetService;
        private static BufferedGraphicsContext _bufferedGraphicsContext;

        private static async Task Main(string[] args)
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
            await _widgetService.Initialize();

            _bufferedGraphicsContext = new BufferedGraphicsContext();
            
            DesktopHandler.Initialize();

            var updateTimer = new QueuedTimer(async x => await Update(), UpdateInterval);
            var renderTimer = new QueuedTimer(async x => await Render(), RenderInterval);

            GC.KeepAlive(updateTimer);
            GC.KeepAlive(renderTimer);

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            SystemEvents.SessionEnded += OnSessionEnded;

            new ManualResetEvent(false).WaitOne();
        }

        private static void Invalidate()
        {
            if (_workerWindow == IntPtr.Zero && !DesktopHandler.TryGetWorkerWindow(out _workerWindow))
            {
                return;
            }

            if (!DesktopHandler.TryGetDeviceContext(_workerWindow, out var deviceContext))
            {
                return;
            }

            var graphics = Graphics.FromHdc(deviceContext);

            using (graphics)
            { 
                graphics.Clear(Color.Teal);
            }
        }

        private static async Task Update()
        {
            UpdateStopWatch.Restart();
            await _widgetService.Update();
            Debug.WriteLine($"Update() took: {UpdateStopWatch.ElapsedMilliseconds}ms");
        }

        private static async Task Render()
        {
            RenderStopWatch.Restart();

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
                await _widgetService.Render(bufferedGraphics.Graphics);
                bufferedGraphics.Render(deviceContext);
                _bufferedGraphicsContext.Invalidate();
            }

            Native.ReleaseDC(_workerWindow, deviceContext);
            Debug.WriteLine($"Render() took: {RenderStopWatch.ElapsedMilliseconds}ms");
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            SystemEvents.SessionEnded -= OnSessionEnded;
        }

        private static async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());

            try
            {
                var fileStream = new FileStream(
                    "./widtop_log.txt",
                    FileMode.OpenOrCreate,
                    FileAccess.Write
                );

                var writer = new StreamWriter(fileStream);

                using (fileStream)
                {
                    using (writer)
                    {
                        await writer.WriteLineAsync($"Unhandled exception occured: {DateTime.Now:yyyy/MMMM/dd HH:mm:ss}");
                        await writer.WriteLineAsync(e.ExceptionObject.ToString());
                    }
                }

                Invalidate();
            }
            catch
            {
                // ignored
            }

            Environment.Exit(1);
        }

        private static async void OnSessionEnded(object s, SessionEndedEventArgs e)
        {
            if (e.Reason != SessionEndReasons.SystemShutdown)
            {
                return;
            }

            await _widgetService.OnShutdown();
        }
    }
}
