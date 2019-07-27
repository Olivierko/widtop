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

        public static bool TryGetWorkerWindow(out IntPtr workerWindow)
        {
            var result = IntPtr.Zero;

            U32.EnumWindows((lpEnumFunc, lParam) =>
            {
                var p = U32.FindWindowEx(
                    lpEnumFunc,
                    IntPtr.Zero,
                    SHELLDLL_WINDOW,
                    null
                );

                if (p != IntPtr.Zero)
                {
                    result = U32.FindWindowEx(
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
            var window = U32.FindWindow(PROGRAM_MANAGER, null);

            U32.SendMessageTimeout(
                window,
                SPAWN_WORKER,
                new IntPtr(0),
                IntPtr.Zero,
                U32.SendMessageTimeoutFlags.SMTO_NORMAL,
                1000,
                out _
            );

            deviceContext = U32.GetDCEx(
                workerWindow,
                IntPtr.Zero,
                U32.DeviceContextValues.DCX_WINDOW | U32.DeviceContextValues.DCX_CACHE | U32.DeviceContextValues.DCX_LOCKWINDOWUPDATE
            );

            return
                deviceContext != IntPtr.Zero &&
                workerWindow != IntPtr.Zero;
        }

        public static void Invalidate()
        {
            var currentMachine = Registry.CurrentUser;
            var controlPanel = currentMachine.OpenSubKey(CONTROL_PANEL);
            var desktop = controlPanel?.OpenSubKey(DESKTOP);

            U32.SystemParametersInfo(
                U32.SPI.SPI_SETDESKWALLPAPER,
                0,
                Convert.ToString(desktop?.GetValue(WALLPAPER)),
                U32.SPIF.SPIF_UPDATEINIFILE
            );
        }
    }
}
