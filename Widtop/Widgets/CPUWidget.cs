﻿// ReSharper disable InconsistentNaming

using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using Widtop.Utility;

namespace Widtop.Widgets
{
    public class CPUWidget : Widget
    {
        private const int BarHeight = 6;
        private const string Name = "CPU";
        private const string CPULoadKey = "CPU Total";
        private const string CPUTemperatureKey = "CPU Package";

        private static Rectangle Area => new Rectangle(WidgetConstants.X + 40, WidgetConstants.Y + 380, 1000, 40);
        private static Font Font => new Font("Agency FB", 18);
        private static SolidBrush TextBrush => new SolidBrush(Color.White);
        private static SolidBrush StatusBrush => new SolidBrush(Color.FromArgb(121, 121, 121));
        private static SolidBrush BarBackgroundBrush => new SolidBrush(Color.FromArgb(31, 31, 31));
        private static SolidBrush BarForegroundBrush => new SolidBrush(Color.White);
        private static StringFormat NameFormat => new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near };
        private static StringFormat StatusFormat => new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
        private static StringFormat ValueFormat => new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Far };

        private Computer _pc;
        private Sample _load;
        private Sample _temperature;

        public override async Task Initialize()
        {
            await Task.Run(() =>
            {
                _load = new Sample();
                _temperature = new Sample();

                _pc = Service.Get<Computer>();
                _pc.CPUEnabled = true;

                _pc.Open();
            });
        }

        public override async Task Update(TimeSpan elapsed)
        {
            await Task.Run(() =>
            {
                lock (_pc)
                {
                    var cpu = _pc.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.CPU);

                    cpu?.Update();
                    var load = cpu?.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Load && x.Name == CPULoadKey)?.Value;
                    var temperature = cpu?.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Temperature && x.Name == CPUTemperatureKey)?.Value;

                    if (load.HasValue)
                    {
                        _load.Add(load.Value);
                    }

                    if (temperature.HasValue)
                    {
                        _temperature.Add(temperature.Value);
                    }
                }
            });
        }

        public override async Task Render(Graphics graphics)
        {
            await Task.Run(() =>
            {
                graphics.DrawString(
                    Name,
                    Font,
                    TextBrush,
                    Area,
                    NameFormat
                );

                graphics.DrawString(
                    $"{_temperature.Avg:0}°C",
                    Font,
                    StatusBrush,
                    Area,
                    StatusFormat
                );

                graphics.DrawString(
                    $"{_load.Avg:0}%",
                    Font,
                    TextBrush,
                    Area,
                    ValueFormat
                );

                graphics.FillRectangle(
                    BarBackgroundBrush,
                    Area.Left,
                    Area.Bottom - BarHeight,
                    Area.Width,
                    BarHeight
                );

                graphics.FillRectangle(
                    BarForegroundBrush,
                    Area.Left,
                    Area.Bottom - BarHeight,
                    Area.Width / 100 * (int)_load.Avg,
                    BarHeight
                );

                _load.Reset();
                _temperature.Reset();
            });
        }
    }
}
