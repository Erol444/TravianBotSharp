using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;
using System.Linq;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Task will close and reopen driver then the next Normal/High priority task has to be executed
    /// </summary>
    public class ReopenDriver : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Wb.Close();

            string previousLog = "";
            do
            {
                await Task.Delay(1000);
                var nextTask = acc.Tasks.ToList().FirstOrDefault();
                var delay = nextTask.ExecuteAt - DateTime.Now;
                int minutes = (int)delay.TotalMinutes;
                if (minutes <= 5) break;
                var log = $"Chrome will reopen in {minutes - 5} mins";
                if (log != previousLog)
                {
                    acc.Logger.Information(log);
                    previousLog = log;
                }
            }
            while (true);
            // Use the same access
            await acc.Wb.Init(acc, false);

            return TaskRes.Executed;
        }
    }
}