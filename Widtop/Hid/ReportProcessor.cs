namespace Widtop.Hid
{
    public abstract class ReportProcessor
    {
        public abstract bool Process(byte[] buffer);
    }
}