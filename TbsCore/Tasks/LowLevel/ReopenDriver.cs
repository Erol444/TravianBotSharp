using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// Task will close and reopen driver then the next Normal/High priority task has to be executed
    /// </summary>
    public class ReopenDriver : BotTask
    {
        /// <summary>
        /// Lowest task priority that will cause the bot to wake up
        /// </summary>
        public TaskPriority LowestPrio { get; set; }
        /// <summary>
        /// Reopen the chrome at specific time
        /// </summary>
        public DateTime? ReopenAt { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Wb.Dispose();


            string previousLog = "";
            TimeSpan nextTask;
            do
            {
                await Task.Delay(1000);
                nextTask = TimeHelper.NextPrioTask(acc, LowestPrio);

                var log = $"Chrome will reopen in {(int)nextTask.TotalMinutes} min";
                if (log != previousLog)
                {
                    acc.Wb.Log(log);
                    previousLog = log;
                }

                if (ReopenAt != null) CheckTime();
            }
            while (TimeSpan.Zero < nextTask);

            // Use the same access
            await acc.Wb.InitSelenium(acc, false);

            return TaskRes.Executed;
        }

        /// <summary>
        /// After ReopenAt, set lowest prio to medium. ReopenAt is used only by SLeep BotTask,
        /// so initially bot will only wakeup when high prio task is ready to be executed, but after
        /// ReopenAt, bot will wakeup on medium prio task as well.
        /// </summary>
        private void CheckTime()
        {
            if (ReopenAt < DateTime.Now)
            {
                LowestPrio = TaskPriority.Medium;
                ReopenAt = null; // So bot won't check it anymore
            }
        }
    }
}
