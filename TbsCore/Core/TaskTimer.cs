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
        private bool flagStopTimer;

        public void ForceTimerStop()
        {
            _acc.Status = Status.Stopping;
            flagStopTimer = true;
        }

        // subTimer is for Tbs's Alzheimer disease
        private readonly Timer _subTimer;

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
            _subTimer = new Timer(500);
            _random = new Random();
            _mainTimer.Elapsed += MainTimerElapsed;
            _subTimer.Elapsed += SubTimerElapsed;
        }

        public void Start()
        {
            if (IsBotRunning) return;
            IsBotRunning = true;
            IsTaskExcuting = false;
            flagStopTimer = false;
            _mainTimer.Start();
            _subTimer.Start();
        }

        public async Task Stop(bool force = false)
        {
            if (!IsBotRunning) return;

            IsBotRunning = false;

            if (!force)
            {
                while (IsTaskExcuting)
                {
                    await Task.Delay(1000);
                }
            }

            _mainTimer.Stop();
            _subTimer.Stop();
        }

        private void MainTimerElapsed(object source, ElapsedEventArgs e) => NewTick();

        private void SubTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // if there is no task and bot still dont know it can execute next task
            if (_acc.Tasks.IsTaskExcuting()) return;
            if (!IsTaskExcuting) return;

            IsTaskExcuting = false;
        }

        private async void NewTick()
        {
            if (!IsBotRunning) return;
            if (IsTaskExcuting) return;
            if (flagStopTimer)
            {
                await Stop(true);
                _acc.Status = Status.Offline;
                return;
            }
            IsTaskExcuting = true;

            if (_acc.Tasks.Count == 0)
            {
                IsTaskExcuting = false;
                return; //No tasks
            }
            // Another task is already in progress. wait
            if (_acc.Tasks.IsTaskExcuting())
            {
                IsTaskExcuting = false;
                return;
            }

            var tasks = _acc.Tasks.GetTasksReady();
            if (tasks.Count == 0)
            {
                NoTasks(_acc);
                IsTaskExcuting = false;
                return;
            }

            BotTask firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.High);
            if (firstTask == null) firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.Medium);
            if (firstTask == null) firstTask = tasks.FirstOrDefault();

            if (firstTask.Stage == TaskStage.Executing)
            {
                IsTaskExcuting = false;
                return;
            }

            firstTask.Stage = TaskStage.Executing;

            //If correct village is selected, otherwise change village
            if (firstTask.Vill != null && firstTask.GetType() != typeof(UpgradeBuilding))
            {
                var active = _acc.Villages.FirstOrDefault(x => x.Active);
                if (active != null && active != firstTask.Vill)
                {
                    await VillageHelper.SwitchVillage(_acc, firstTask.Vill.Id);
                }
            }
            await TaskExecutor.Execute(_acc, firstTask);
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
            _subTimer.Dispose();
        }
    }
}