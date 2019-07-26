using System;

namespace Widtop.Hid
{
    public class ErrorReportProcessor : ReportProcessor
    {
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

            Log("Received a report with error status.");

            return true;
        }
    }
}