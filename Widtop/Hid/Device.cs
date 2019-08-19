namespace Widtop.Hid
{
    public abstract class Device
    {
        public abstract string Name { get; }
        public abstract int VendorId { get; }
        public abstract int ProductId { get; }
        public abstract int ReceiverId { get; }

        public virtual ReportProcessor[] Processors { get; protected set; } = new ReportProcessor[0];

        public virtual void OnInitialize(Connector connector) { }

        public virtual void OnConnected() { }

        public virtual bool MatchesVirtual(string devicePath)
        {
            return true;
        }

        public virtual bool MatchesPhysical(string devicePath)
        {
            return true;
        }

        public void OnReportReceived(byte[] buffer)
        {
            foreach (var processor in Processors)
            {
                if (processor.Process(buffer))
                {
                    break;
                }
            }
        }
    }
}
