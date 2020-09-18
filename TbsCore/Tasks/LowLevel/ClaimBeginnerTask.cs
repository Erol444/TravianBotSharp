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
            var script = "var quests = document.getElementById('mentorTaskList').children;";
            script += $"for(var i=0;i<quests.length;i++) if(quests[i].getAttribute('data-questid')=='{questId}') quests[i].click();";
            await DriverHelper.ExecuteScript(acc, script);

            var collectButton = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.GetAttributeValue("questid", "") == questId);
            acc.Wb.Driver.ExecuteScript($"document.getElementById('{collectButton.Id}').click();");
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
