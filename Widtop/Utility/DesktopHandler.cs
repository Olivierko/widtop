// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using Microsoft.Win32;

namespace Widtop.Utility
{
    public static class DesktopHandler
    {
        private const string PROGRAM_MANAGER = "Progman";
        private const string WORKER_WINDOW = "WorkerW";
        private const string SHELLDLL_WINDOW = "SHELLDLL_DefView";
        private const string CONTROL_PANEL = "Control Panel";
        private const string DESKTOP = "Desktop";
        private const string WALLPAPER = "Wallpaper";
        private const uint SPAWN_WORKER = 0x052C;

        public static void Initialize()
        {
            var window = Native.FindWindow(PROGRAM_MANAGER, null);
            Native.SendMessage(window, SPAWN_WORKER, (IntPtr)0x0000000D, (IntPtr)0);
            Native.SendMessage(window, SPAWN_WORKER, (IntPtr)0x0000000D, (IntPtr)1);
        }

        public static void Invalidate()
        {
            var currentMachine = Registry.CurrentUser;
            var controlPanel = currentMachine.OpenSubKey(CONTROL_PANEL);
            var desktop = controlPanel?.OpenSubKey(DESKTOP);

            Native.SystemParametersInfo(
                Native.SPI.SPI_SETDESKWALLPAPER,
                0,
                Convert.ToString(desktop?.GetValue(WALLPAPER)),
                Native.SPIF.SPIF_UPDATEINIFILE
            );
        }

        public static bool TryGetWorkerWindow(out IntPtr workerWindow)
        {
            var result = IntPtr.Zero;

            Native.EnumWindows((lpEnumFunc, lParam) =>
            {
                var p = Native.FindWindowEx(
                    lpEnumFunc,
                    IntPtr.Zero,
                    SHELLDLL_WINDOW,
                    null
                );

                if (p != IntPtr.Zero)
                {
                    result = Native.FindWindowEx(
                        IntPtr.Zero,
                        lpEnumFunc,
                        WORKER_WINDOW,
                        null
                    );
                }

                return true;
            }, IntPtr.Zero);

            workerWindow = result;
            return workerWindow != IntPtr.Zero;
        }

        public static bool TryGetDeviceContext(IntPtr workerWindow, out IntPtr deviceContext)
        {
            var window = Native.FindWindow(PROGRAM_MANAGER, null);

            Native.SendMessageTimeout(
                window,
                SPAWN_WORKER,
                IntPtr.Zero,
                IntPtr.Zero,
                Native.SendMessageTimeoutFlags.SMTO_NORMAL,
                1000,
                out _
            );

            deviceContext = Native.GetDCEx(
                workerWindow,
                IntPtr.Zero,
                Native.DeviceContextValues.DCX_WINDOW | Native.DeviceContextValues.DCX_CACHE | Native.DeviceContextValues.DCX_LOCKWINDOWUPDATE
            );

            return
                deviceContext != IntPtr.Zero &&
                workerWindow != IntPtr.Zero;
        }
    }
}
