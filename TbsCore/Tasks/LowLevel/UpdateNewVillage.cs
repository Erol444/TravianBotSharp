using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpdateNewVillage : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            TaskExecutor.RemoveSameTasksForVillage(acc, Vill, typeof(UpdateDorf1), this);
            TaskExecutor.RemoveSameTasksForVillage(acc, Vill, typeof(UpdateDorf2), this);

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php"); // Update dorf1
            await Task.Delay(AccountHelper.Delay());
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php"); // Update dorf2

            // On new village import the building tasks
            if (string.IsNullOrEmpty(acc.NewVillages.BuildingTasksLocationNewVillage))
            {
                DefaultConfigurations.FarmVillagePlan(acc, Vill);
            }
            else
            {
                IoHelperCore.AddBuildTasksFromFile(acc, Vill, acc.NewVillages.BuildingTasksLocationNewVillage);
            }

            await UpdateTroopsResearchedAndLevels(acc);
            await UpdateTroopsTraining(acc);

            var firstTroop = TroopsHelper.TribeFirstTroop(acc.AccInfo.Tribe);
            Vill.Troops.TroopToTrain = firstTroop;
            Vill.Troops.Researched.Add(firstTroop);

            return TaskRes.Executed;
        }

        // Copied from UpdateTroops BotTask
        public async Task UpdateTroopsResearchedAndLevels(Account acc)
        {
            if (acc.AccInfo.PlusAccount)
            {
                // From overview we get all researched troops and their levels
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf3.php?s=5&su=2");
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
                UpdateResearchedTroops(Vill);
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
            foreach(var trainingBuilding in trainingBuildings)
            {
                var building = Vill.Build.Buildings.FirstOrDefault(x => x.Type == trainingBuilding);
                if (building == null) continue;

                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={building.Id}");

                var ct = TroopsParser.GetTroopsCurrentlyTraining(acc.Wb.Html);
                switch (building.Type)
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
            }
        }
        private void UpdateResearchedTroops(Village vill)
        {
            if (vill.Troops.Levels.Count > 0) vill.Troops.Researched = vill.Troops.Levels.Select(x => x.Troop).ToList();
        }
    }
}
