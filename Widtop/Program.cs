using System.Threading;

namespace Widtop
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new ManualResetEvent(false).WaitOne();
        }
    }
}
