using System;

namespace Widtop.Hid
{
    public class BatteryReportProcessor : ReportProcessor
    {
        public delegate void BatteryUpdate(double volt, BatteryVoltageStatus status, double discharge, decimal percentage);

        public event BatteryUpdate BatteryUpdated;

        public BatteryReportProcessor(Action<string> log) : base(log)
        {
        }

        public override bool Process(byte[] buffer)
        {
            var matchesFeature =
                buffer.Length >= 8 &&
                buffer[0] == (byte)ReportSize.Long &&
                buffer[2] == (byte)ReportType.Battery;

            if (!matchesFeature)
            {
                return false;
            }

            var milliVolt = (buffer[4] << 8) | buffer[5];
            var status = (BatteryVoltageStatus)buffer[6];

            var volt = milliVolt / 1000d;

            DischargeCurve.Discharge matchingCurve = null;
            for (var index = 0; index < DischargeCurve.Values.Length; index++)
            {
                var curve = DischargeCurve.Values[index];

                if (curve.Volts <= volt)
                {
                    matchingCurve = curve;
                    break;
                }
            }

            if (matchingCurve == null)
            {
                Log($"Failed to resolve discharge curve for voltage: {volt}, falling back to lowest.");
                matchingCurve = DischargeCurve.Values[DischargeCurve.Values.Length - 1];
            }

            var maxMinutes = DischargeCurve.Values[DischargeCurve.Values.Length - 1].Minutes;
            var percentage = 100m - decimal.Divide(matchingCurve.Minutes, maxMinutes) * 100;

            Log("#########################");
            Log("Battery report received:");
            Log($"Battery voltage: {volt}");
            Log($"Battery status: {status}");
            Log($"Battery discharge: {matchingCurve.mWh} mWh");
            Log($"Percentage: ~{percentage}% (based on minutes)");
            Log("#########################");

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
