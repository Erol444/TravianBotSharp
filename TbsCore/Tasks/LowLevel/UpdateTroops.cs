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
    public class UpdateTroops : BotTask
    {
        /// <summary>
        /// When new village is found by the bot, it should firstly check barracks, then  stable and then workshop,
        /// to see which troops are researched
        /// </summary>
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            // If we have Plus account, just check that.
            if (acc.AccInfo.PlusAccount)
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf3.php?s=5&su=2");
                OverviewParser.UpdateTroopsLevels(acc.Wb.Html, ref acc);
                // We have updated all villages at the same time. No need to continue.
                acc.Tasks.RemoveAll(x => x.GetType() == typeof(UpdateTroops));
                return TaskRes.Executed;
            }

            var smithy = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Smithy);
            if (smithy != null)
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={smithy.Id}");
                Vill.Troops.Levels = TroopsParser.GetTroopLevels(acc.Wb.Html);
                UpdateResearchedTroops(Vill);
                return TaskRes.Executed;
            }

            for (int i = 0; i < 3; i++)
            {
                //var building = GetBuilding(i);
                //await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={building.Id}");
                // TODO: parse content
            }
            return TaskRes.Executed;
        }

        private void UpdateResearchedTroops(Village vill)
        {
            if (vill.Troops.Levels.Count > 0) vill.Troops.Researched = vill.Troops.Levels.Select(x => x.Troop).ToList();
        }

        private Models.ResourceModels.Building GetBuilding(int check)
        {
            switch (check)
            {
                case 1: return Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Barracks);
                case 2: return Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Stable);
                case 3: return Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Workshop);
                default: return null;
            }
        }
    }
}
