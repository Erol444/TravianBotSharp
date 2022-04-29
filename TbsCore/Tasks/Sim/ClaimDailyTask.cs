using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Sim
{
    public class ClaimDailyTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.TTwars:
                    await NavigationHelper.ToDorf2(acc);
                    await DriverHelper.ExecuteScript(acc, "Travian.Game.Quest.openTodoListDialog('', true);");
                    var script = "var dialog = document.getElementById('dialogContent');";
                    script += "dialog.getElementsByClassName('active')[0].click();";
                    await DriverHelper.ExecuteScript(acc, script);
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    await NavigationHelper.MainNavigate(acc, NavigationHelper.MainNavigationButton.DailyQuests);
                    await DriverHelper.ClickByClassName(acc, "rewardReady");
                    break;
            }
            await DriverHelper.ClickByClassName(acc, "questButtonGainReward");

            return TaskRes.Executed;
        }
    }
}