using System;

namespace Widtop.Hid
{
    public abstract class ReportProcessor
    {
        protected Action<string> Log { get; }

        protected ReportProcessor(Action<string> log)
        {
            Log = log;
        }

        public abstract bool Process(byte[] buffer);
    }
}