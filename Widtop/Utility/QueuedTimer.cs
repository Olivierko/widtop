using System;
using System.Diagnostics;
using System.Threading;

namespace Widtop.Utility
{
    public class QueuedTimer : IDisposable
    {
        private readonly int _interval;
        private readonly Timer _timer;
        private readonly Stopwatch _stopwatch;
        private readonly TimerCallback _callback;

        private bool _stopped;

        public QueuedTimer(TimerCallback callback, int interval) : this(callback, 0, interval)
        {
        }

        public QueuedTimer(TimerCallback callback, int dueTime, int interval)
        {
            _callback = callback;
            _interval = interval;
            _stopwatch = new Stopwatch();
            _timer = new Timer(Callback, null, dueTime, interval);
            _stopped = false;
        }

        private void Callback(object state)
        {
            try
            {
                _stopwatch.Restart();
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _callback.Invoke(state);
                _stopwatch.Stop();
            }
            finally
            {
                if (!_stopped)
                {
                    var dueTime = Math.Max(
                        0,
                        _interval - _stopwatch.ElapsedMilliseconds
                    );

                    _timer.Change(dueTime, _interval);
                }
            }
        }

        public void Trigger()
        {
            _stopped = false;
            _timer.Change(0, _interval);
        }

        public void Stop()
        {
            _stopped = true;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
