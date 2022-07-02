using System;
using System.Timers;

namespace MainCore.Services
{
    public sealed class TimerManager : ITimerManager
    {
        public TimerManager(IDatabaseEvent databaseEvent)
        {
            _mainTimer = new Timer(500);
            _mainTimer.Elapsed += MainTimer_Elapsed;
            _databaseEvent = databaseEvent;
        }

        private void MainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _databaseEvent.OnTaskExecuted();
        }

        public void Dispose()
        {
            _mainTimer.Dispose();
        }

        public void Start()
        {
            _mainTimer.Start();
        }

        private readonly Timer _mainTimer;
        private readonly IDatabaseEvent _databaseEvent;
    }
}