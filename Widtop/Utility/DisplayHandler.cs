// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Widtop.Utility
{
    public static class DisplayHandler
    {
        public class Display
        {
            public IntPtr Handle { get; }
            public uint Index { get; }
            public int X { get; }
            public int Y { get; }
            public int Width { get; }
            public int Height { get; }
            public string Name { get; }

            public Display(IntPtr handle, uint index, int x, int y, int width, int height, string name)
            {
                Handle = handle;
                Index = index;
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Name = name;
            }
        }

        public static List<Display> GetAll()
        {
            var displays = new List<Display>();

            uint index = 0;
            Native.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            (IntPtr hMonitor, IntPtr hdcMonitor, ref Native.Rectangle lprcMonitor, IntPtr data) =>
            {
                var monitorInfo = new Native.MonitorInfoEx();
                monitorInfo.Size = (uint)Marshal.SizeOf(monitorInfo);

                var success = Native.GetMonitorInfo(hMonitor, ref monitorInfo);

                if (!success)
                {
                    index++;
                    return true;
                }

                var width = monitorInfo.Monitor.Right - monitorInfo.Monitor.Left;
                var height = monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top;

                var display = new Display(
                    hMonitor,
                    index,
                    monitorInfo.WorkArea.Left,
                    monitorInfo.WorkArea.Top,
                    width,
                    height,
                    monitorInfo.DeviceName
                );

                displays.Add(display);

                index++;
                return true;
            }, IntPtr.Zero);

            return displays;
        }
    }
}
