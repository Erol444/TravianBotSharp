using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Farming
{
    public class SendFLs : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            Random rnd = new Random();
            int totalSec = 0;
            await Task.Delay(AccountHelper.Delay(acc) / 3);

            foreach (var fl in acc.Farming.FL)
            {
                // Check interval of the farmlist
                if (fl.Interval > 1)
                {
                    fl.IntervalCounter++;
                    if (fl.IntervalCounter < fl.Interval) continue;
                    fl.IntervalCounter = 0;
                }

                if (fl.Enabled)
                {
                    acc.Tasks.Add(new SendFarmlist() { ExecuteAt = DateTime.Now.AddSeconds(totalSec), FL = fl, Priority = TaskPriority.High });
                    totalSec += rnd.Next(5, 13);
                }
            }

            int nextExecuteSec = 100;
            nextExecuteSec = rnd.Next(acc.Farming.MinInterval, acc.Farming.MaxInterval);

            acc.Tasks.Add(new SendFLs() { ExecuteAt = DateTime.Now.AddSeconds(nextExecuteSec), Priority = TaskPriority.High });

            return TaskRes.Executed;
        }
    }
}