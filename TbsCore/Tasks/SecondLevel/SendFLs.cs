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
                    TaskExecutor.AddTask(acc, new SendFarmlist() { ExecuteAt = DateTime.Now.AddSeconds(totalSec), FL = fl });
                    switch (acc.AccInfo.ServerVersion)
                    {
                        case Classificator.ServerVersionEnum.T4_4:
                            // For TTWars, you need 30sec delay between each FL send
                            totalSec += rnd.Next(acc.Farming.MinInterval, acc.Farming.MaxInterval);
                            break;
                        case Classificator.ServerVersionEnum.T4_5:
                            totalSec += rnd.Next(5, 13);
                            break;
                    }
                }
            }

            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    TaskExecutor.AddTask(acc, new SendFLs() { ExecuteAt = DateTime.Now.AddSeconds(totalSec) });
                    break;
                case Classificator.ServerVersionEnum.T4_5:
                    var nextSend = rnd.Next(acc.Farming.MinInterval, acc.Farming.MaxInterval);
                    TaskExecutor.AddTask(acc, new SendFLs() { ExecuteAt = DateTime.Now.AddSeconds(nextSend) });
                    break;
            }
            
            return TaskRes.Executed;
        }
    }
}
