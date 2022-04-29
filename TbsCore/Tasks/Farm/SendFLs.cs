using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Tasks.SecondLevel
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
                    switch (acc.AccInfo.ServerVersion)
                    {
                        case Classificator.ServerVersionEnum.TTwars:
                            // For TTWars, you need 30sec delay between each FL send
                            totalSec += rnd.Next(acc.Farming.MinInterval, acc.Farming.MaxInterval);
                            break;

                        case Classificator.ServerVersionEnum.T4_5:
                            totalSec += rnd.Next(5, 13);
                            break;
                    }
                }
            }

            int nextExecuteSec = 100;
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.TTwars:
                    nextExecuteSec = totalSec;
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    nextExecuteSec = rnd.Next(acc.Farming.MinInterval, acc.Farming.MaxInterval);
                    break;
            }
            acc.Tasks.Add(new SendFLs() { ExecuteAt = DateTime.Now.AddSeconds(nextExecuteSec), Priority = TaskPriority.High });

            return TaskRes.Executed;
        }
    }
}