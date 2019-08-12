using Widtop.Hid;

namespace Widtop.Logitech
{
    public class BatteryReportProcessor : ReportProcessor
    {
        public delegate void BatteryUpdate(double volt, BatteryVoltageStatus status, double discharge, decimal percentage);

        private readonly LightspeedDevice _device;
        public event BatteryUpdate BatteryUpdated;

        public BatteryReportProcessor(LightspeedDevice device)
        {
            _device = device;
        }

        public override bool Process(byte[] buffer)
        {
            var matches =
                buffer.Length >= 8 &&
                buffer[0] == (byte)ReportSize.Long &&
                buffer[2] == (byte)ReportType.Battery;

            if (!matches)
            {
                return false;
            }

            var milliVolt = (buffer[4] << 8) | buffer[5];
            var status = (BatteryVoltageStatus)buffer[6];

            var volt = milliVolt / 1000d;

            Discharge matchingCurve = null;
            for (var index = 0; index < _device.DischargeCurve.Length; index++)
            {
                var curve = _device.DischargeCurve[index];

                if (curve.Volts <= volt)
                {
                    matchingCurve = curve;
                    break;
                }
            }

            if (matchingCurve == null)
            {
                // fallback to last curve when no match
                matchingCurve = _device.DischargeCurve[_device.DischargeCurve.Length - 1];
            }

            var maxMinutes = _device.DischargeCurve[_device.DischargeCurve.Length - 1].Minutes;
            var percentage = 100m - decimal.Divide(matchingCurve.Minutes, maxMinutes) * 100;

            BatteryUpdated?.Invoke(
                volt, 
                status, 
                matchingCurve.mWh, 
                percentage
            );

            return true;
        }
    }
}
