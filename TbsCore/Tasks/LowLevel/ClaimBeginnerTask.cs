using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.SideBarModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ClaimBeginnerTask : BotTask
    {
        public Quest QuestToClaim { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            var script = $"document.getElementById('mentorTaskList').querySelector('[data-questid=\"{this.QuestToClaim.Id}\"]').click();";
            await DriverHelper.ExecuteScript(acc, script);
            await Task.Delay(AccountHelper.Delay() * 2);

            string buttonId = "";
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_5:
                buttonId = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.GetAttributeValue("questid", "") == this.QuestToClaim.Id).Id;
                    break;

                case Classificator.ServerVersionEnum.T4_4:
                buttonId = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("questButtonNext"))?.Id;
                    break;
            }

            acc.Wb.Driver.FindElementById(buttonId).Click();
            return TaskRes.Executed;
        }
    }
}
