using Widtop.Hid;

namespace Widtop.Logitech
{
    public class ErrorReportProcessor : ReportProcessor
    {
        public delegate void ErrorReceive();

        public event ErrorReceive ErrorReceived;

        public override bool Process(byte[] buffer)
        {
            var matchesFeature =
                buffer.Length >= 3 &&
                buffer[2] == (byte)ReportType.Error;

            if (!matchesFeature)
            {
                return false;
            }

            ErrorReceived?.Invoke();

            return true;
        }
    }
}