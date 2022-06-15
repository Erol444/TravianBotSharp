using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Sim
{
    public class ClaimDailyTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.MainNavigate(acc, NavigationHelper.MainNavigationButton.DailyQuests);
            await DriverHelper.ClickByClassName(acc, "rewardReady");

            await DriverHelper.ClickByClassName(acc, "questButtonGainReward");

            return TaskRes.Executed;
        }
    }
}