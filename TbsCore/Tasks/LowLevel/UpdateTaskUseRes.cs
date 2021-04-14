using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
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

            // Train settlers
            // Copy from UpgradeBuilding task
            await TaskExecutor.PageLoaded(acc);

            // Check if residence is getting upgraded to level 10 => train settlers
            var cbResidence = Vill.Build
                .CurrentlyBuilding
                .FirstOrDefault(x => x.Building == Classificator.BuildingEnum.Residence && x.Level == 10);

            if (cbResidence != null &&
                acc.NewVillages.AutoSettleNewVillages &&
                Vill.Troops.Settlers == 0)
            {
                TaskExecutor.AddTaskIfNotExistInVillage(acc, Vill,
                    new TrainSettlers()
                    {
                        ExecuteAt = cbResidence.Duration.AddSeconds(5),
                        Vill = Vill,
                        // For high speed servers, you want to train settlers asap
                        Priority = 1000 < acc.AccInfo.ServerSpeed ? TaskPriority.High : TaskPriority.Medium,
                    });
            }

            return TaskRes.Executed;
        }
    }
}