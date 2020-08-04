using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// This task changes proxy (and restarts selenium driver) for the account and sets the next proxy change, if there are multiple access'.
    /// </summary>
    public class ChangeProxy : BotTask
    {
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            acc.Wb.Close();

            // Wait some time between the proxy switching.
            Random rnd = new Random();
            var seconds = rnd.Next(30, 600); // TODO: make this configurable (wait time between switches)
            //TODO: also check for high priority tasks (attacking/deffending). If there is one, don't wait so long!

            this.DurationCounter = -2 * seconds;
            await Task.Delay(seconds * 1000);

            acc.Wb.InitSelenium(acc);

            return TaskRes.Executed;
        }
    }
}
