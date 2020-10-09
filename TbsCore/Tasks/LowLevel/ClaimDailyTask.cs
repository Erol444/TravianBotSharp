using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ClaimDailyTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await DriverHelper.ExecuteScript(acc, "Travian.Game.Quest.openTodoListDialog('', true);");

            var script = "var dialog = document.getElementById('dialogContent');";
            script += "dialog.getElementsByClassName('active')[0].click();";
            await DriverHelper.ExecuteScript(acc, script);

            script = "document.getElementsByClassName('questButtonGainReward')[0].click();";
            await DriverHelper.ExecuteScript(acc, script);

            return TaskRes.Executed;
        }
    }
}
