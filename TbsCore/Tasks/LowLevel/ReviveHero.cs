using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ReviveHero : BotTask
    {
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php");

            //heroRegeneration
            var reviveButton = htmlDoc.GetElementbyId("heroRegeneration");
            if (reviveButton == null)
            {
                this.ErrorMessage = "No revive button!";
                return TaskRes.Executed;
            }
            if (reviveButton.HasClass("green"))
            {
                wb.ExecuteScript("document.getElementById('heroRegeneration').click()"); //revive hero
                return TaskRes.Executed;
            }
            else
            {
                //no resources?
                this.NextExecute = DateTime.Now.AddMinutes(10);
                return TaskRes.Executed;
            }
        }
    }
}
