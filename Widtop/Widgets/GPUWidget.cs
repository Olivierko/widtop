﻿// ReSharper disable InconsistentNaming

using System.Drawing;
using System.Linq;
using OpenHardwareMonitor.Hardware;

namespace Widtop.Widgets
{
    public class GPUWidget : Widget
    {
        private const int BarHeight = 6;
        private const string Name = "GPU";
        private const string GPUCoreKey = "GPU Core";

        private static Rectangle Area => new Rectangle(2560 + 240, 240 + 140, 400, 40);
        private static Font Font => new Font("Agency FB", 18);
        private static SolidBrush TextBrush => new SolidBrush(Color.White);
        private static SolidBrush StatusBrush => new SolidBrush(Color.FromArgb(121, 121, 121));
        private static SolidBrush BarBackgroundBrush => new SolidBrush(Color.FromArgb(31, 31, 31));
        private static SolidBrush BarForegroundBrush => new SolidBrush(Color.White);
        private static StringFormat NameFormat => new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near };
        private static StringFormat StatusFormat => new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
        private static StringFormat ValueFormat => new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Far };

        private Computer _pc;

        private float? _load;
        private float? _temperature;

        public override void Initialize()
        {
            _pc = Service.Get<Computer>();
            _pc.GPUEnabled = true;
        }

        public override void Update()
        {
            lock (_pc)
            {
                _pc.Open();

                var gpu = _pc.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.GpuNvidia || x.HardwareType == HardwareType.GpuAti);

                _load = gpu?.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Load && x.Name == GPUCoreKey)?.Value;
                _temperature = gpu?.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Temperature && x.Name == GPUCoreKey)?.Value;

                _pc.Close();
            }
        }

        public override void Render(Graphics graphics)
        {
            graphics.DrawString(
                Name,
                Font,
                TextBrush,
                Area,
                NameFormat
            );

            graphics.DrawString(
                $"{_temperature ?? -1}°C",
                Font,
                StatusBrush,
                Area,
                StatusFormat
            );

            graphics.DrawString(
                $"{_load ?? -1:0}%",
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
                (int)(Area.Width / 100 * _load ?? 0),
                BarHeight
            );
        }
    }
}
