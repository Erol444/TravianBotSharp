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

        long restartTasksCounter = 0;

        private async void NewTick()
        {
            // Dirty hack. TODO fix the code, so building/troops filling tasks won't fail by themselves
            restartTasksCounter++;
            if (restartTasksCounter > 7200)
            {
                restartTasksCounter = 0;
                foreach (var vill in acc.Villages)
                {
                    if (!TroopsHelper.EverythingFilled(acc, vill)) TroopsHelper.ReStartTroopTraining(acc, vill);
                    BuildingHelper.ReStartBuilding(acc, vill);
                }
            }

            if (acc.Tasks.Count == 0) return; //No tasks

            // Another task is already in progress. wait
            var taskInProgress = acc.Tasks.FirstOrDefault(x => x.Stage != TaskStage.Start);
            if (taskInProgress != null) return;

            var tasks = acc.Tasks.Where(x => x.ExecuteAt <= DateTime.Now).ToList();
            if (tasks.Count == 0) return; // No tasks yet

            BotTask firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.High);
            if (firstTask == null) firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.Medium);
            if (firstTask == null) firstTask = tasks.FirstOrDefault();

            firstTask.Stage = TaskStage.Executing;

            //If correct village is selected, otherwise change village
            if (firstTask.Vill != null)
            {
                var active = acc.Villages.FirstOrDefault(x => x.Active);
                // && active != firstTask.Vill
                if (active != null) //error handling
                {
                    await VillageHelper.SwitchVillage(acc, firstTask.Vill.Id);
                }
            }
            _ = TaskExecutor.Execute(acc, firstTask);
        }
    }
}