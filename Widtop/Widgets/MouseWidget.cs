using System;
using System.Drawing;
using System.Threading.Tasks;
using Widtop.Hid;
using Widtop.Logitech;

namespace Widtop.Widgets
{
    public class MouseWidget : Widget
    {
        private const int BarHeight = 6;

        private static Rectangle Area => new Rectangle(WidgetConstants.X + 240, WidgetConstants.Y + 20, 400, 40);
        private static Font Font => new Font("Agency FB", 18);
        private static SolidBrush TextBrush => new SolidBrush(Color.White);
        private static SolidBrush StatusBrush => new SolidBrush(Color.FromArgb(121, 121, 121));
        private static SolidBrush BarBackgroundBrush => new SolidBrush(Color.FromArgb(31, 31, 31));
        private static SolidBrush BarForegroundBrush => new SolidBrush(Color.White);
        private static StringFormat NameFormat => new StringFormat{ LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near };
        private static StringFormat StatusFormat => new StringFormat{ LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
        private static StringFormat ValueFormat => new StringFormat{ LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Far };

        private LightspeedDevice _device;

        private string GetStatusString()
        {
            switch (_device.Status)
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

        public override async Task Initialize()
        {
            await Task.Run(() =>
            {
                _device = new G703();

                var connector = new Connector(_device);
                connector.Initialize();
            });
        }

        public override async Task Render(Graphics graphics)
        {
            await Task.Run(() =>
            {
                var status = GetStatusString();

                graphics.DrawString(
                    _device.Name,
                    Font,
                    TextBrush,
                    Area,
                    NameFormat
                );

                graphics.DrawString(
                    status,
                    Font,
                    StatusBrush,
                    Area,
                    StatusFormat
                );

                graphics.DrawString(
                    $"{_device.Battery ?? -1:0}%",
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
                    (int)(Area.Width / 100 * _device.Battery ?? 0),
                    BarHeight
                );
            });
        }
    }
}
