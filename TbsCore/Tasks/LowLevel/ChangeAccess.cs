using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// This task changes access (and restarts selenium driver) for the account and sets the next access change, if there are multiple access'.
    /// </summary>
    public class ChangeAccess : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Wb.Dispose();

            //TODO: make this configurable (wait time between switches)
            var rand = new Random();
            int sleepSec = rand.Next(30, 600);
            var sleepEnd = DateTime.Now.AddSeconds(sleepSec);

            await TimeHelper.SleepUntilPrioTask(acc, TaskPriority.High, sleepEnd);

            await acc.Wb.InitSelenium(acc);

            // Remove all other ChangeAccess tasks
            acc.Tasks.RemoveAll(x => x.GetType() == typeof(ChangeAccess) && x != this);

            var nextProxyChange = TimeHelper.GetNextProxyChange(acc);
            if (nextProxyChange != TimeSpan.MaxValue)
            {
                this.NextExecute = DateTime.Now + nextProxyChange;
            }

            return TaskRes.Executed;
        }
    }
}
