using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ChangeVillageName : BotTask
    {
        public List<(int, string)> ChangeList { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/spieler.php?s=2");

            if(acc.Wb.Html.GetElementbyId("PlayerProfileEditor") == null)
            {
                // Sitter. Can't change the name of the village. TODO: check if sitter before
                // creating the task.
                return TaskRes.Executed;
            }

            foreach (var change in ChangeList)
            {
                var script = $"document.getElementsByName('dname[{change.Item1}]=')[0].value='{change.Item2}'";
                wb.ExecuteScript(script); //insert new name into the textbox
            }

            await Task.Delay(AccountHelper.Delay());

            wb.FindElementById("PlayerProfileEditor").Click();

            return TaskRes.Executed;
        }
    }
}
