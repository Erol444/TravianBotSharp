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
            await Task.Delay(AccountHelper.Delay());
            TroopsHelper.ReStartResearchAndImprovement(acc, Vill);
            await Task.Delay(AccountHelper.Delay());

            // Building task
            BuildingHelper.ReStartBuilding(acc, Vill);
            await Task.Delay(AccountHelper.Delay());
            BuildingHelper.ReStartDemolishing(acc, Vill);
            await Task.Delay(AccountHelper.Delay());

            // Celebration task
            AccountHelper.ReStartCelebration(acc, Vill);
            return TaskRes.Executed;
        }
    }
}