using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Others
{
    public class SetCapital : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            if (!Vill.Build.Buildings.Any(x => x.Type == Classificator.BuildingEnum.Palace))
            {
                // TODO: Check for residence, if it exists demolish it and build palace
                acc.Logger.Information("Palace was not found in the village!");
                return TaskRes.Executed;
            }

            // Go into palace
            await NavigationHelper.ToGovernmentBuilding(acc, Vill, NavigationHelper.ResidenceTab.Managenment);

            acc.Logger.Warning("Setting capital isn't supported in T4.5 yet!");

            return TaskRes.Executed;
        }
    }
}