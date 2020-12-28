using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ReviveHero : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php");

            //heroRegeneration
            var reviveButton = acc.Wb.Html.GetElementbyId("heroRegeneration");
            if (reviveButton == null)
            {
                acc.Wb.Log("No revive button!");
                return TaskRes.Executed;
            }
            if (reviveButton.HasClass("green"))
            {
                wb.FindElementById("heroRegeneration").Click();
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
