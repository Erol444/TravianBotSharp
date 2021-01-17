using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

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

            await TimeHelper.SleepUntilPrioTask(acc, LowestPrio, ReopenAt);

            // Use the same access
            await acc.Wb.InitSelenium(acc, false);

            return TaskRes.Executed;
        }
    }
}
