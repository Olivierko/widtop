using System;

namespace Widtop.Hid
{
    public class ErrorReportProcessor : ReportProcessor
    {
        public delegate void ErrorReceive();

        public event ErrorReceive ErrorReceived;

        public ErrorReportProcessor(Action<string> log) : base(log)
        {
        }

        public override bool Process(byte[] buffer)
        {
            var matchesFeature =
                buffer.Length >= 3 &&
                buffer[2] == (byte)ReportType.Error;

            if (!matchesFeature)
            {
                return false;
            }

            Log($"Received a report with error status, buffer: {string.Join(" ", buffer)}");

            ErrorReceived?.Invoke();

            return true;
        }
    }
}