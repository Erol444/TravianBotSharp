using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Files.Tasks.SecondLevel
{
    public class SendFLs : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            Random rnd = new Random();
            int totalSec = 0;

            foreach (var fl in acc.Farming.FL)
            {
                // Check interval of the farmlist
                if (fl.Interval > 1)
                {
                    if (fl.IntervalCounter <= fl.Interval)
                    {
                        fl.IntervalCounter++;
                        continue;
                    }
                    fl.IntervalCounter = 0;
                }

                if (fl.Enabled)
                {
                    //TODO: do some logic with counters, so not all FLs get sent with same period
                    TaskExecutor.AddTask(acc, new SendFarmlist() { ExecuteAt = DateTime.Now.AddSeconds(totalSec), FL = fl });
                    totalSec += rnd.Next(acc.Farming.MinInterval, acc.Farming.MaxInterval);
                }
            }
            TaskExecutor.AddTask(acc, new SendFLs() { ExecuteAt = DateTime.Now.AddSeconds(totalSec) });
            return TaskRes.Executed;
        }
    }
}
