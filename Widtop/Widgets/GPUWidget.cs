// ReSharper disable InconsistentNaming

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

        private readonly Computer _pc;

        private float? _load;
        private float? _temperature;

        public GPUWidget(Computer pc)
        {
            _pc = pc;
        }

        public override void Update()
        {
            lock (_pc)
            {
                _pc.Open();

                var gpu = _pc.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);

                _load = gpu.Sensors.First(x => x.SensorType == SensorType.Load && x.Name == GPUCoreKey).Value;
                _temperature = gpu.Sensors.First(x => x.SensorType == SensorType.Temperature && x.Name == GPUCoreKey).Value;

                _pc.Close();
            }
        }

        public override void Render(Buffer buffer, Graphics graphics)
        {
            if (!buffer.Matches(Area, out var localArea))
            {
                return;
            }

            graphics.DrawString(
                Name,
                Font,
                TextBrush,
                localArea,
                NameFormat
            );

            graphics.DrawString(
                $"{_temperature ?? -1}°C",
                Font,
                StatusBrush,
                localArea,
                StatusFormat
            );

            graphics.DrawString(
                $"{_load ?? -1:0}%",
                Font,
                TextBrush,
                localArea,
                ValueFormat
            );

            graphics.FillRectangle(
                BarBackgroundBrush,
                localArea.Left,
                localArea.Bottom - BarHeight,
                localArea.Width,
                BarHeight
            );

            graphics.FillRectangle(
                BarForegroundBrush,
                localArea.Left,
                localArea.Bottom - BarHeight,
                (int)(localArea.Width / 100 * _load ?? 0),
                BarHeight
            );
        }
    }
}
