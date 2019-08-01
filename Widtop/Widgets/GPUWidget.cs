// ReSharper disable InconsistentNaming

using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using OpenHardwareMonitor.Hardware;

namespace Widtop.Widgets
{
    public class GPUWidget : Widget
    {
        private const int BarHeight = 6;
        private const string Name = "GPU";
        private const string GPUCoreKey = "GPU Core";
        private const string ImageResourceName = "Widtop.Content.gpu_widget_image.png";

        private static Rectangle Area => new Rectangle(880, 910, 400, 40);
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
        private Image _image;

        public GPUWidget(Computer pc)
        {
            _pc = pc;
        }

        public override void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(ImageResourceName))
            {
                _image = Image.FromStream(stream ?? throw new InvalidOperationException());
            }
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

        public override void Render(Graphics graphics)
        {
            //graphics.FillRectangle(new SolidBrush(Color.Blue), Area);
            //graphics.DrawImage(_image, Area.Left, Area.Top);

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
