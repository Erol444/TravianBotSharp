using System.Collections.Generic;
using System.Timers;
using MainCore.Services.Interface;

namespace MainCore.Services.Implementations
{
    public sealed class TimerManager : ITimerManager
    {
        public TimerManager(EventManager EventManager)
        {
            _eventManager = EventManager;
        }

        public void Dispose()
        {
            foreach (var timer in _dictTimer.Values)
            {
                timer.Dispose();
            }
        }

        public void Start(int index)
        {
            if (!_dictTimer.ContainsKey(index))
            {
                var timer = new Timer(100) { AutoReset = false };
                timer.Elapsed += (sender, e) =>
                {
                    try
                    {
                        _eventManager.OnTaskExecuted(index);
                        timer.Start();
                    }
                    catch
                    {
                        return;
                    }
                };
                _dictTimer.Add(index, timer);
                _dictTimer[index].Start();
            }
        }

        private readonly Dictionary<int, Timer> _dictTimer = new();
        private readonly EventManager _eventManager;
    }
}