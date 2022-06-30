using System;
using System.Timers;

namespace MainCore.Services
{
    public sealed class TimerManager : ITimerManager
    {
        public event Action TaskExecute;

        public TimerManager()
        {
            _mainTimer = new Timer(500);
            _mainTimer.Elapsed += mainTimer_Elapsed;
        }

        private void mainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TaskExecute?.Invoke();
        }

        public void Dispose()
        {
            _mainTimer.Dispose();
        }

        public void Start()
        {
            _mainTimer.Start();
        }

        public void Stop()
        {
            _mainTimer.Stop();
        }

        private readonly Timer _mainTimer;
    }
}