using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Widtop.Hid;

namespace Widtop.Widgets
{
    public class MouseWidget : Widget
    {
        private const string ImageResourceName = "Widtop.Content.mouse_widget_image.png";

        private static Font Font => new Font("Agency FB", 18);
        private static SolidBrush Brush => new SolidBrush(Color.White);
        private static Point ImageAnchor => new Point(880, 770);
        private static Point PercentageAnchor => Point.Add(ImageAnchor, new Size(28, 2));
        private static Point StatusAnchor => Point.Add(PercentageAnchor, new Size(50, 0));

        private decimal _batteryPercentage;
        private Image _image;
        private BatteryVoltageStatus _status;
        private LightspeedConnector _lightspeedConnector;

        private static void Log(string message) { }

        private void OnBatteryUpdated(double volt, BatteryVoltageStatus status, double discharge, decimal percentage)
        {
            _status = status;
            _batteryPercentage = percentage;
        }

        public override void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(ImageResourceName))
            {
                _image = Image.FromStream(stream ?? throw new InvalidOperationException());
            }

            var batteryProcessor = new BatteryReportProcessor(Log);
            batteryProcessor.BatteryUpdated += OnBatteryUpdated;

            var processors = new List<ReportProcessor>
            {
                batteryProcessor,
                new ErrorReportProcessor(Log)
            };

            _lightspeedConnector = new LightspeedConnector(
                Log, 
                processors
            );

            _lightspeedConnector.Initialize();
        }

        public override void Render(Graphics graphics)
        {
            string status;
            switch (_status)
            {
                case BatteryVoltageStatus.BATTERY_VOLTAGE_STATUS_DISCHARGING:
                    status = "<<";
                    break;
                case BatteryVoltageStatus.BATTERY_VOLTAGE_STATUS_CHARGING:
                    status = "‡";
                    break;
                case BatteryVoltageStatus.BATTERY_VOLTAGE_STATUS_WIRELESS_CHARGING:
                    status = "∞";
                    break;
                case BatteryVoltageStatus.BATTERY_VOLTAGE_STATUS_FULLY_CHARGED:
                    status = "√";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            graphics.DrawImage(_image, ImageAnchor);
            graphics.DrawString($"{_batteryPercentage:0}%", Font, Brush, PercentageAnchor);
            graphics.DrawString($"{status}", Font, Brush, StatusAnchor);
        }
    }
}
