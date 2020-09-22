using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// This task changes access (and restarts selenium driver) for the account and sets the next access change, if there are multiple access'.
    /// </summary>
    public class ChangeAccess : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Wb.Close();

            //TODO: make this configurable (wait time between switches)

            // Wait some time (1min) between the proxy switching.
            var sleep = new Sleep()
            {
                AutoSleep = false,
                MinSleepSec = 10,
                MaxSleepSec = 60,
            };
            // sleep will stop if there is a high priority task
            await sleep.Execute(acc);

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
