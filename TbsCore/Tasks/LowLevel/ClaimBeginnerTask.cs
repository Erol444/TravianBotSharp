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
            var questId = QuestId(QuestToClaim);
            var script = $"document.getElementById('mentorTaskList').querySelector('[data-questid=\"{questId}\"]').click();";
            await DriverHelper.ExecuteScript(acc, script);
            await Task.Delay(AccountHelper.Delay() * 2);

            string buttonId = "";
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_5:
                buttonId = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.GetAttributeValue("questid", "") == questId).Id;
                    break;

                case Classificator.ServerVersionEnum.T4_4:
                buttonId = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("questButtonNext"))?.Id;
                    break;
            }

            acc.Wb.Driver.ExecuteScript($"document.getElementById('{buttonId}').click();");
            return TaskRes.Executed;
        }

        private string QuestId(Quest q)
        {
            string ret = q.category.ToString();
            ret += "_";
            var num = q.level;
            ret += (num >= 10 ? num.ToString() : "0" + num);
            return ret;
        }
    }
}
