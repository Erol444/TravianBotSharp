using MainCore.Services.Interface;
using System.Collections.Generic;
using System.Timers;

namespace MainCore.Services.Implementations
{
    public sealed class TimerManager : ITimerManager
    {
        public TimerManager(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public void Shutdown()
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
                        _eventManager.OnTaskExecute(index);
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
        private readonly IEventManager _eventManager;
    }
}