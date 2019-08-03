using System;
using System.Collections.Generic;
using System.Drawing;
using Widtop.Hid;

namespace Widtop.Widgets
{
    // TODO: refactor LightspeedConnector to resolve mouse model itself and create instance per model with appropriate data for discharge, name etc
    public class MouseWidget : Widget
    {
        private const int BarHeight = 6;
        private const string Name = "G703";

        private static Rectangle Area => new Rectangle(2560 + 240, 240 + 20, 400, 40);
        private static Font Font => new Font("Agency FB", 18);
        private static SolidBrush TextBrush => new SolidBrush(Color.White);
        private static SolidBrush StatusBrush => new SolidBrush(Color.FromArgb(121, 121, 121));
        private static SolidBrush BarBackgroundBrush => new SolidBrush(Color.FromArgb(31, 31, 31));
        private static SolidBrush BarForegroundBrush => new SolidBrush(Color.White);
        private static StringFormat NameFormat => new StringFormat{ LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near };
        private static StringFormat StatusFormat => new StringFormat{ LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
        private static StringFormat ValueFormat => new StringFormat{ LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Far };

        private decimal? _batteryPercentage;
        private BatteryVoltageStatus? _status;
        private LightspeedConnector _lightspeedConnector;

        private static void Log(string message) { }

        private string GetStatusString()
        {
            switch (_status)
            {
                case null:
                    return "?";
                case BatteryVoltageStatus.BATTERY_VOLTAGE_STATUS_DISCHARGING:
                    return "<<";
                case BatteryVoltageStatus.BATTERY_VOLTAGE_STATUS_CHARGING:
                    return ">>";
                case BatteryVoltageStatus.BATTERY_VOLTAGE_STATUS_WIRELESS_CHARGING:
                    return "∞";
                case BatteryVoltageStatus.BATTERY_VOLTAGE_STATUS_FULLY_CHARGED:
                    return "√";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnBatteryUpdated(double volt, BatteryVoltageStatus status, double discharge, decimal percentage)
        {
            _status = status;
            _batteryPercentage = percentage;
        }

        private void OnErrorReceived()
        {
            _lightspeedConnector.Reset();
        }

        public override void Initialize()
        {
            var batteryProcessor = new BatteryReportProcessor(Log);
            batteryProcessor.BatteryUpdated += OnBatteryUpdated;

            var errorProcessor = new ErrorReportProcessor(Log);
            errorProcessor.ErrorReceived += OnErrorReceived;

            var processors = new List<ReportProcessor>
            {
                batteryProcessor,
                errorProcessor
            };

            _lightspeedConnector = new LightspeedConnector(
                Log, 
                processors
            );

            _lightspeedConnector.Initialize();
        }

        public override void Render(Buffer buffer, Graphics graphics)
        {
            if (!buffer.Matches(Area, out var localArea))
            {
                return;
            }

            var status = GetStatusString();
            
            graphics.DrawString(
                Name, 
                Font, 
                TextBrush,
                localArea, 
                NameFormat
            );

            graphics.DrawString(
                status,
                Font,
                StatusBrush,
                localArea,
                StatusFormat
            );

            graphics.DrawString(
                $"{_batteryPercentage ?? -1:0}%", 
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
                (int)(localArea.Width / 100 * _batteryPercentage ?? 0),
                BarHeight
            );
        }
    }
}
