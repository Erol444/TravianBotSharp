using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// This task changes access (and restarts selenium driver) for the account and sets the next access change, if there are multiple access'.
    /// </summary>
    public class ChangeAccess : BotTask
    {
        private int WaitSecMin;
        private int WaitSecMax;

        public ChangeAccess(int min = 30, int max = 600, DateTime executeAt = default) : base(null, executeAt, TaskPriority.High)
        {
            WaitSecMin = min;
            WaitSecMax = max;
        }

        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Wb.Dispose();

            var rand = new Random();
            var sleepSec = rand.Next(WaitSecMin, WaitSecMax);
            var sleepEnd = DateTime.Now.AddSeconds(sleepSec);

            await TimeHelper.SleepUntilPrioTask(acc, TaskPriority.High, sleepEnd);

            await acc.Wb.InitSelenium(acc);

            var nextProxyChange = TimeHelper.GetNextProxyChange(acc);
            if (nextProxyChange != TimeSpan.MaxValue)
            {
                this.NextExecute = DateTime.Now + nextProxyChange;
            }

            return TaskRes.Executed;
        }
    }
}