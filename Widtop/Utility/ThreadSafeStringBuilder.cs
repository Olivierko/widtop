using System.Text;
using System.Threading;

namespace Widtop.Utility
{
    public class ThreadSafeStringBuilder
    {
        private readonly ReaderWriterLockSlim _lock;
        private readonly StringBuilder _stringBuilder;

        public ThreadSafeStringBuilder()
        {
            _lock = new ReaderWriterLockSlim();
            _stringBuilder = new StringBuilder();
        }

        public void AppendLine(string value)
        {
            try
            {
                _lock.EnterWriteLock();
                _stringBuilder.AppendLine(value);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            try
            {
                _lock.EnterWriteLock();
                _stringBuilder.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public override string ToString()
        {
            try
            {
                _lock.EnterReadLock();
                return _stringBuilder.ToString();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
