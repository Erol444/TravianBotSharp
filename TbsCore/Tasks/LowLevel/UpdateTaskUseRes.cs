using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    internal class UpdateTaskUseRes : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            // Troops task
            TroopsHelper.ReStartTroopTraining(acc, Vill);
            await Task.Delay(AccountHelper.Delay(acc));
            TroopsHelper.ReStartResearchAndImprovement(acc, Vill);
            await Task.Delay(AccountHelper.Delay(acc));

            // Building task
            BuildingHelper.ReStartBuilding(acc, Vill);
            await Task.Delay(AccountHelper.Delay(acc));
            BuildingHelper.ReStartDemolishing(acc, Vill);
            await Task.Delay(AccountHelper.Delay(acc));

            // Celebration task
            AccountHelper.ReStartCelebration(acc, Vill);
            return TaskRes.Executed;
        }
    }
}