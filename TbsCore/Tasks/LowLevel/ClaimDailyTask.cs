using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ClaimDailyTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");

            await DriverHelper.ExecuteScript(acc, "Travian.Game.Quest.openTodoListDialog('', true);");

            var script = "var dialog = document.getElementById('dialogContent');";
            script += "dialog.getElementsByClassName('active')[0].click();";
            await DriverHelper.ExecuteScript(acc, script);

            await acc.Wb.Driver.FindElementByClassName("questButtonGainReward").Click(acc);

            return TaskRes.Executed;
        }
    }
}
