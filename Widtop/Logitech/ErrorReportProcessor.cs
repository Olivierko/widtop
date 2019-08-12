using Widtop.Hid;

namespace Widtop.Logitech
{
    public class ErrorReportProcessor : ReportProcessor
    {
        public delegate void ErrorReceive();

        public event ErrorReceive ErrorReceived;

        public override bool Process(byte[] buffer)
        {
            var matches =
                buffer.Length >= 3 &&
                buffer[2] == (byte)ReportType.Error;

            if (!matches)
            {
                return false;
            }

            ErrorReceived?.Invoke();

            return true;
        }
    }
}