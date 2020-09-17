using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class Sleep : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var htmlDoc = acc.Wb.Html;
            var wb = acc.Wb.Driver;
            var rand = new Random();
            int sleepSec = rand.Next(acc.Settings.Time.MinSleep * 60, acc.Settings.Time.MaxSleep * 60);
            await Task.Delay(sleepSec * 1000);

            this.NextExecute = DateTime.Now + TimeHelper.GetWorkTime(acc);

            return TaskRes.Executed;
        }
    }
}
