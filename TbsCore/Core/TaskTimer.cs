using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TbsCore.Helpers;
using TbsCore.Tasks;
using TbsCore.Tasks.LowLevel;
using static TbsCore.Tasks.BotTask;

namespace TbsCore.Models.AccModels
{
    public sealed class TaskTimer : IDisposable
    {
        private readonly Random _random;
        private readonly Account _acc;
        private readonly Timer _mainTimer;

        private long _isTaskExcuting;

        public bool IsTaskExcuting
        {
            get
            {
                return System.Threading.Interlocked.Read(ref _isTaskExcuting) == 1;
            }
            private set
            {
                System.Threading.Interlocked.Exchange(ref _isTaskExcuting, Convert.ToInt64(value));
            }
        }

        private long _isBotRunning;

        public bool IsBotRunning
        {
            get
            {
                return System.Threading.Interlocked.Read(ref _isBotRunning) == 1;
            }
            private set
            {
                System.Threading.Interlocked.Exchange(ref _isBotRunning, Convert.ToInt64(value));
            }
        }

        public TaskTimer(Account Account)
        {
            _acc = Account;
            _mainTimer = new Timer(500);
            _random = new Random();
            _mainTimer.Elapsed += MainTimerElapsed;
        }

        public void Start()
        {
            if (IsBotRunning) return;
            IsBotRunning = true;
            IsTaskExcuting = false;
            _mainTimer.Start();
        }

        public void Stop()
        {
            if (!IsBotRunning) return;

            IsBotRunning = false;
            _mainTimer.Stop();

            var currentTask = _acc.Tasks.CurrentTask;
            if (currentTask != null) currentTask.StopFlag = true;
            _acc.Status = Status.Pausing;
        }

        public async Task WaitStop()
        {
            await Task.Run(() => { while (IsTaskExcuting) { } });
            _acc.Status = Status.Paused;
        }

        private void MainTimerElapsed(object source, ElapsedEventArgs e) => NewTick();

        private async void NewTick()
        {
            if (!IsBotRunning) return;
            if (IsTaskExcuting) return;
            IsTaskExcuting = true;

            if (_acc.Tasks.Count == 0)
            {
                IsTaskExcuting = false;
                return; //No tasks
            }

            var task = _acc.Tasks.FirstTask;
            if (task.ExecuteAt > DateTime.Now)
            {
                NoTasks(_acc);
                IsTaskExcuting = false;
                return;
            }

            task.Stage = TaskStage.Executing;

            //If correct village is selected, otherwise change village
            if (task.Vill != null && task.GetType() != typeof(UpgradeBuilding))
            {
                var active = _acc.Villages.FirstOrDefault(x => x.Active);
                if (active != null && active != task.Vill)
                {
                    await VillageHelper.SwitchVillage(_acc, task.Vill.Id);
                }
            }

            await TaskExecutor.Execute(_acc, task);

            IsTaskExcuting = false;
        }

        private void NoTasks(Account _acc)
        {
            if (_acc.Settings.AutoCloseDriver)
            {
                var nextTask = _acc.Tasks.ToList().FirstOrDefault();
                var delay = TimeSpan.FromMinutes(5);
                if (nextTask == null || nextTask.ExecuteAt - DateTime.Now > delay)
                {
                    _acc.Tasks.Add(new TaskSleep()
                    {
                        ExecuteAt = DateTime.Now,
                    });
                    return;
                }
            }

            if (_acc.Settings.AutoRandomTasks)
            {
                var nextTask = _acc.Tasks.ToList().FirstOrDefault();
                var delay = TimeSpan.FromMinutes(20);
                if (nextTask == null || nextTask.ExecuteAt - DateTime.Now > delay)
                {
                    _acc.Tasks.Add(new RandomTask()
                    {
                        ExecuteAt = DateTime.Now.AddSeconds(_random.Next(60, 600)),
                    });
                    return;
                }
            }
        }

        public void Dispose()
        {
            _mainTimer.Dispose();
        }
    }
}