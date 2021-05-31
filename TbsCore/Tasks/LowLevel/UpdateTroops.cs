﻿using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.LowLevel
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
                await VersionHelper.Navigate(acc, "/dorf3.php?s=5&su=2", "/village/statistics/troops?su=2");

                OverviewParser.UpdateTroopsLevels(acc.Wb.Html, ref acc);
                // We have updated all villages at the same time. No need to continue.
                acc.Tasks.Remove(typeof(UpdateTroops));
                return TaskRes.Executed;
            }

            var smithy = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Smithy);
            if (smithy != null)
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={smithy.Id}");
                Vill.Troops.Levels = TroopsParser.GetTroopLevels(acc.Wb.Html);
                TroopsHelper.UpdateResearchedTroops(Vill);
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

        private Building GetBuilding(int check)
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