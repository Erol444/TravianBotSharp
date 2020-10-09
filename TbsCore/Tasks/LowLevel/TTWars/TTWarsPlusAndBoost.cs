using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    //since "extend automatically" doesn't work on TTWars, this task will automatically prolong plus account / +25% resource boost
    public class TTWarsPlusAndBoost : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            var leftBar = acc.Wb.Html.GetElementbyId("sidebarBeforeContent");
            var button = leftBar.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));
            if (button == null)
            {
                return TaskRes.Executed;
            }
            var buttonid = button.GetAttributeValue("id", "");
            acc.Wb.Driver.ExecuteScript($"document.getElementById('{buttonid}').click()"); //boost production

            return TaskRes.Executed;
        }
    }
}
