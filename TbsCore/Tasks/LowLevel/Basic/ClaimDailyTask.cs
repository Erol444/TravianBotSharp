using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    public class ClaimDailyTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");

            await DriverHelper.ExecuteScript(acc, "Travian.Game.Quest.openTodoListDialog('', true);");

            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var script = "var dialog = document.getElementById('dialogContent');";
                    script += "dialog.getElementsByClassName('active')[0].click();";
                    await DriverHelper.ExecuteScript(acc, script);
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    await DriverHelper.ExecuteScript(acc, "document.getElementsByClassName('rewardReady')[0].click();");
                    break;
            }

            await DriverHelper.ExecuteScript(acc, "document.getElementsByClassName('questButtonGainReward')[0].click();");

            return TaskRes.Executed;
        }
    }
}