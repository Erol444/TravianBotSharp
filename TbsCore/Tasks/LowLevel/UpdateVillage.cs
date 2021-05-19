﻿using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.TravianData;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpdateVillage : BotTask
    {
        /// <summary>
        /// If village is new, we want to import the building tasks to the village
        /// </summary>
        public bool ImportTasks { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            TaskExecutor.RemoveTaskTypes(acc, typeof(UpdateDorf1), Vill, this);
            TaskExecutor.RemoveTaskTypes(acc, typeof(UpdateDorf2), Vill, this);

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php"); // Update dorf1
            await Task.Delay(AccountHelper.Delay());
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php"); // Update dorf2

            // On new village import the building tasks
            if (ImportTasks && !string.IsNullOrEmpty(acc.NewVillages.BuildingTasksLocationNewVillage))
            {
                IoHelperCore.AddBuildTasksFromFile(acc, Vill, acc.NewVillages.BuildingTasksLocationNewVillage);
            }

            await UpdateTroopsResearchedAndLevels(acc);

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");
            await Task.Delay(AccountHelper.Delay());
            await UpdateTroopsTraining(acc);

            var firstTroop = TroopsData.TribeFirstTroop(acc.AccInfo.Tribe);
            Vill.Troops.TroopToTrain = firstTroop;
            Vill.Troops.Researched.Add(firstTroop);

            if (await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.TownHall))
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
                await VersionHelper.Navigate(acc, "/dorf3.php?s=5&su=2", "/village/statistics/troops?su=2");
                
                OverviewParser.UpdateTroopsLevels(acc.Wb.Html, ref acc);
                // We have updated all villages at the same time. No need to continue.
                acc.Tasks.RemoveAll(x => x.GetType() == typeof(UpdateTroops));
                return;
            }

            var smithy = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Smithy);
            if (smithy != null)
            {
                // If smithy exists, we get all researched troops and their levels
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={smithy.Id}");
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
                if (!await VillageHelper.EnterBuilding(acc, Vill, trainingBuilding)) continue;

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
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");
                await Task.Delay(AccountHelper.Delay());
            }
        }

    }
}
