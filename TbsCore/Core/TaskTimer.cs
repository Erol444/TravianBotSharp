using System;
using System.Linq;
using System.Timers;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;
using static TravBotSharp.Files.Tasks.BotTask;

namespace TravBotSharp.Files.Models.AccModels
{
    public class TaskTimer : IDisposable
    {
        private readonly Account acc;

        public TaskTimer(Account account)
        {
            acc = account;
            Timer = new Timer(500);
            Timer.Elapsed += TimerElapsed;
            Start();
        }

        private Timer Timer { get; }

        public void Dispose()
        {
            Timer.Dispose();
        }

        public bool? IsBotRunning()
        {
            return Timer.Enabled;
        }

        public void Start()
        {
            Timer.Start();
            Timer.Enabled = true;
            Timer.AutoReset = true;
        }

        public void Stop()
        {
            Timer.Stop();
            Timer.Enabled = false;
        }

        private void TimerElapsed(object source, ElapsedEventArgs e)
        {
            NewTick();
        }

        private async void NewTick()
        {
            try
            {
                if (acc.Tasks.Count == 0) return; //No tasks

                // Another task is already in progress. wait
                if (acc.Tasks.Any(x => x.Stage != TaskStage.Start)) return;

                var tasks = acc.Tasks.Where(x => x.ExecuteAt <= DateTime.Now).ToList();
                if (tasks.Count == 0)
                {
                    NoTasks(acc);
                    return;
                }

                var firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.High);
                if (firstTask == null) firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.Medium);
                if (firstTask == null) firstTask = tasks.FirstOrDefault();

                firstTask.Stage = TaskStage.Executing;

                //If correct village is selected, otherwise change village
                if (firstTask.Vill != null)
                {
                    var active = acc.Villages.FirstOrDefault(x => x.Active);
                    if (active != null && active != firstTask.Vill)
                        await VillageHelper.SwitchVillage(acc, firstTask.Vill.Id);
                }

                await TaskExecutor.Execute(acc, firstTask);
            }
            catch (Exception e)
            {
                acc?.Wb?.Log($"Error in TaskTimer! {e.Message}\n{e.StackTrace}");
            }
        }

        private void NoTasks(Account acc)
        {
            BotTask task = null;
            var updateVill = acc.Villages.FirstOrDefault(x => x.Timings.NextVillRefresh < DateTime.Now);

            if (updateVill != null)
            {
                // Update the village
                task = new UpdateDorf1 {Vill = updateVill};
            }
            else if (acc.Settings.AutoCloseDriver &&
                     TimeSpan.FromMinutes(5) < TimeHelper.NextPrioTask(acc, TaskPriority.Medium))
            {
                // Auto close chrome and reopen when there is a high/normal prio BotTask
                task = new ReopenDriver();
                ((ReopenDriver) task).LowestPrio = TaskPriority.Medium;
            }
            else if (acc.Settings.AutoRandomTasks)
            {
                task = new RandomTask();
            }

            if (task != null)
            {
                task.ExecuteAt = DateTime.Now;
                task.Priority = TaskPriority.Low;
                TaskExecutor.AddTask(acc, task);
            }
        }
    }
}