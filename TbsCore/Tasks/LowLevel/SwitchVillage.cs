
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SwitchVillage : BotTask
    {
        public new DateTime ExecuteAt = DateTime.MinValue;
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            string str = "?";
            if (acc.Wb.CurrentUrl.Contains("?")) str = "&";
            var url = $"{acc.Wb.CurrentUrl}{str}newdid={this.vill.Id}";
            await acc.Wb.Navigate(url);
            return TaskRes.Executed;
        }
    }
}
