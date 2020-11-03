using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// Task will close and reopen driver then the next Normal/High priority task has to be executed
    /// </summary>
    public class ReopenDriver : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Wb.Close();

            TimeSpan nextTask;
            do
            {
                await Task.Delay(1000);
                nextTask = TimeHelper.NextNormalOrHighPrioTask(acc);
                this.Message = $"Chrome will reopen in {nextTask.TotalMinutes} min";
            }
            while (nextTask > TimeSpan.Zero);

            // Use the same access
            await acc.Wb.InitSelenium(acc, false);

            return TaskRes.Executed;
        }
    }
}
