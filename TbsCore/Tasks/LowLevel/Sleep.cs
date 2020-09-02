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
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            var rand = new Random();
            int sleepSec = rand.Next(acc.Settings.Time.MinSleep * 60, acc.Settings.Time.MaxSleep * 60);
            // Set durationCounter low enough so it won't interrupt sleep (durationCounter decreases by 1 every 500ms)
            this.DurationCounter = -2 * sleepSec;
            await Task.Delay(sleepSec * 1000);

            this.NextExecute = DateTime.Now + TimeHelper.GetWorkTime(acc);

            return TaskRes.Executed;
        }
    }
}
