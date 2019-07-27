// ReSharper disable InconsistentNaming

using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using OpenHardwareMonitor.Hardware;

namespace Widtop.Widgets
{
    public class CPUWidget : Widget
    {
        private const string ImageResourceName = "Widtop.Content.cpu_widget_image.png";
        private const string CPULoadKey = "CPU Total";
        private const string CPUTemperatureKey = "CPU Package";

        private static Font Font => new Font("Agency FB", 18);
        private static SolidBrush Brush => new SolidBrush(Color.White);
        private static Point ImageAnchor => new Point(880, 800);
        private static Point LoadAnchor => Point.Add(ImageAnchor, new Size(28, 2));
        private static Point TemperatureAnchor => Point.Add(LoadAnchor, new Size(50, 0));

        private readonly Computer _pc;

        private float? _load;
        private float? _temperature;
        private Image _image;

        public CPUWidget(Computer pc)
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

                var cpu = _pc.Hardware.First(x => x.HardwareType == HardwareType.CPU);

                _load = cpu.Sensors.First(x => x.SensorType == SensorType.Load && x.Name == CPULoadKey).Value;
                _temperature = cpu.Sensors.First(x => x.SensorType == SensorType.Temperature && x.Name == CPUTemperatureKey).Value;

                _pc.Close();
            }
        }

        public override void Render(Graphics graphics)
        {
            graphics.DrawImage(_image, ImageAnchor);
            graphics.DrawString($"{_load:0}%", Font, Brush, LoadAnchor);
            graphics.DrawString($"{_temperature ?? -1}°C", Font, Brush, TemperatureAnchor);
        }
    }
}
