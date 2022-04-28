using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.TravianData;
using TbsCore.Parsers;

namespace TbsCore.Tasks.LowLevel
{
    public class UpdateVillage : BotTask
    {
        /// <summary>
        /// If village is new, we want to import the building tasks to the village
        /// </summary>
        public bool ImportTasks { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Tasks.Remove(typeof(UpdateDorf1), Vill, thisTask: this);
            acc.Tasks.Remove(typeof(UpdateDorf2), Vill, thisTask: this);

            await NavigationHelper.ToDorf1(acc);
            await NavigationHelper.ToDorf2(acc);

            // On new village import the building tasks
            if (ImportTasks && !string.IsNullOrEmpty(acc.NewVillages.BuildingTasksLocationNewVillage))
            {
                IoHelperCore.AddBuildTasksFromFile(acc, Vill, acc.NewVillages.BuildingTasksLocationNewVillage);
            }

            await UpdateTroopsResearchedAndLevels(acc);

            await Task.Delay(AccountHelper.Delay(acc));
            await UpdateTroopsTraining(acc);

            var firstTroop = TroopsData.TribeFirstTroop(acc.AccInfo.Tribe);
            Vill.Troops.TroopToTrain = firstTroop;
            Vill.Troops.Researched.Add(firstTroop);

            if (await NavigationHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.TownHall))
            {
                // Village has town hall, parse celebration duration
                Vill.Expansion.CelebrationEnd = TimeParser.GetCelebrationTime(acc.Wb.Html);
            }

            return TaskRes.Executed;
        }

        // Copied from UpdateTroops BotTask
        public async Task UpdateTroopsResearchedAndLevels(Account acc)
        {
            if (acc.AccInfo.PlusAccount)
            {
                // From overview we get all researched troops and their levels
                await NavigationHelper.ToOverview(acc, NavigationHelper.OverviewTab.Troops, NavigationHelper.TroopOverview.Smithy);

                OverviewParser.UpdateTroopsLevels(acc.Wb.Html, ref acc);
                // We have updated all villages at the same time. No need to continue.
                acc.Tasks.Remove(typeof(UpdateTroops));
                return;
            }

            // If smithy exists, we get all researched troops and their levels
            if (await NavigationHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Smithy))
            {
                Vill.Troops.Levels = TroopsParser.GetTroopLevels(acc.Wb.Html);
                TroopsHelper.UpdateResearchedTroops(Vill);
                return;
            }
        }

        private readonly Classificator.BuildingEnum[] trainingBuildings = new Classificator.BuildingEnum[] {
            Classificator.BuildingEnum.Barracks,
            Classificator.BuildingEnum.Stable,
            Classificator.BuildingEnum.Workshop,
            Classificator.BuildingEnum.GreatBarracks,
            Classificator.BuildingEnum.GreatStable
        };

        public async Task UpdateTroopsTraining(Account acc)
        {
            foreach (var trainingBuilding in trainingBuildings)
            {
                if (!await NavigationHelper.EnterBuilding(acc, Vill, trainingBuilding)) continue;

                // Mark troops that user can train in building as researched
                TroopsHelper.UpdateTroopsResearched(Vill, acc.Wb.Html);

                var ct = TroopsParser.GetTroopsCurrentlyTraining(acc.Wb.Html);
                switch (trainingBuilding)
                {
                    case Classificator.BuildingEnum.Barracks:
                        Vill.Troops.CurrentlyTraining.Barracks = ct;
                        break;

                    case Classificator.BuildingEnum.Stable:
                        Vill.Troops.CurrentlyTraining.Stable = ct;
                        break;

                    case Classificator.BuildingEnum.GreatBarracks:
                        Vill.Troops.CurrentlyTraining.GB = ct;
                        break;

                    case Classificator.BuildingEnum.GreatStable:
                        Vill.Troops.CurrentlyTraining.GS = ct;
                        break;

                    case Classificator.BuildingEnum.Workshop:
                        Vill.Troops.CurrentlyTraining.Workshop = ct;
                        break;
                }
                await Task.Delay(AccountHelper.Delay(acc));
            }
        }
    }
}