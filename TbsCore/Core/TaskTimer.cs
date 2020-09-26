using System;
using System.Linq;
using System.Timers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;
using static TravBotSharp.Files.Tasks.BotTask;

namespace TravBotSharp.Files.Models.AccModels
{
    public class TaskTimer
    {
        private readonly Account acc;
        private Timer Timer { get; set; }
        public bool? IsBotRunning() => Timer.Enabled;
        public TaskTimer(Account account)
        {
            acc = account;
            Timer = new Timer(500);
            Timer.Elapsed += TimerElapsed;
            Start();
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

        private void TimerElapsed(Object source, ElapsedEventArgs e)
        {
            try
            {
                NewTick();
            }
            catch(Exception excep)
            {

            }
        }

        private async void NewTick()
        {
            // Dirty hack. TODO fix the code, so building/troops filling tasks won't fail by themselves
            //restartTasksCounter++;
            //if (restartTasksCounter > 7200)
            //{
            //    restartTasksCounter = 0;
            //    foreach (var vill in acc.Villages)
            //    {
            //        if (!TroopsHelper.EverythingFilled(acc, vill)) TroopsHelper.ReStartTroopTraining(acc, vill);
            //        BuildingHelper.ReStartBuilding(acc, vill);
            //    }
            //}

            if (acc.Tasks.Count == 0) return; //No tasks

            // Another task is already in progress. wait
            var taskInProgress = acc.Tasks.FirstOrDefault(x => x.Stage != TaskStage.Start);
            if (taskInProgress != null) return;

            var tasks = acc.Tasks.Where(x => x.ExecuteAt <= DateTime.Now).ToList();
            if (tasks.Count == 0)
            {
                NoTasks(acc);
                return;
            }

            BotTask firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.High);
            if (firstTask == null) firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.Medium);
            if (firstTask == null) firstTask = tasks.FirstOrDefault();

            firstTask.Stage = TaskStage.Executing;

            //If correct village is selected, otherwise change village
            if (firstTask.Vill != null)
            {
                var active = acc.Villages.FirstOrDefault(x => x.Active);
                if (active != null && active != firstTask.Vill)
                {
                    await VillageHelper.SwitchVillage(acc, firstTask.Vill.Id);
                }
            }
            _ = TaskExecutor.Execute(acc, firstTask);
        }

        private void NoTasks(Account acc)
        {
            BotTask task = null;
            var updateVill = acc.Villages.FirstOrDefault(x => x.Timings.LastVillRefresh + TimeSpan.FromMinutes(30) < DateTime.Now);

            if (updateVill != null)
            {
                // Update the village
                task = new UpdateDorf1
                {
                    Vill = updateVill
                };
            }
            else if (acc.Hero.Settings.AutoRefreshInfo && acc.Settings.Timing.LastHeroRefresh + TimeSpan.FromMinutes(30) < DateTime.Now)
            {
                task = new HeroUpdateInfo();
            }
            else if (acc.Settings.AutoCloseDriver &&
                TimeHelper.NextNormalOrHighPrioTask(acc) > TimeSpan.FromMinutes(5))
            {
                // Auto close chrome and reopen when there is a high/normal prio BotTask
                task = new ReopenDriver();
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