using System;
using System.Linq;
using System.Timers;
using TbsCore.Helpers;
using TbsCore.Tasks;
using TbsCore.Tasks.LowLevel;
using static TbsCore.Tasks.BotTask;

namespace TbsCore.Models.AccModels
{
    public sealed class TaskTimer : IDisposable
    {
        private readonly Account _acc;
        private readonly Timer _mainTimer;

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
            _mainTimer.Elapsed += MainTimerElapsed;
            _subTimer.Elapsed += SubTimerElapsed;
        }

        public void Start()
        {
            IsBotRunning = true;
            IsTaskExcuting = false;
            _mainTimer.Start();
        }

        public void Stop()
        {
            IsBotRunning = false;
            IsTaskExcuting = false;
            _mainTimer.Stop();
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
            if (firstTask.Vill != null)
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
            BotTask task = null;

            if (_acc.Settings.AutoCloseDriver &&
                TimeSpan.FromMinutes(5) < TimeHelper.NextPrioTask(_acc, TaskPriority.Medium))
            {
                // Auto close chrome and reopen when there is a high/normal prio BotTask
                task = new ReopenDriver();
                ((ReopenDriver)task).LowestPrio = TaskPriority.Medium;
            }
            else if (_acc.Settings.AutoRandomTasks)
            {
                task = new RandomTask();
            }

            if (task != null)
            {
                task.ExecuteAt = DateTime.Now;
                task.Priority = TaskPriority.Low;
                _acc.Tasks.Add(task);
            }
        }

        public void Dispose()
        {
            _mainTimer.Dispose();
            _subTimer.Dispose();
        }
    }
}