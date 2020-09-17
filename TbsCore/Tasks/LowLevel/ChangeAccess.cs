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

            //TODO: also check for high priority tasks (attacking/deffending). If there is one, don't wait so long!
            //TODO: make this configurable (wait time between switches)

            // Wait some time between the proxy switching.
            await Task.Delay(AccountHelper.Delay() * 5);

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
