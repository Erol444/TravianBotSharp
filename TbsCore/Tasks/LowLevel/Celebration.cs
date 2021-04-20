﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class Celebration : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.TownHall))
                return TaskRes.Executed;

            var celebrationEnd = TimeParser.GetCelebrationTime(acc.Wb.Html);
            if (DateTime.Now <= celebrationEnd)
            {
                // We already have a celebration running
                Vill.Expansion.CelebrationEnd = celebrationEnd;
                NextExecute = celebrationEnd;
                return TaskRes.Executed;
            }

            var buildingNode = acc.Wb.Html.GetElementbyId("build");
            var (_, level) = InfrastructureParser.UpgradeBuildingGetInfo(buildingNode);

            var bigCeleb = Vill.Expansion.Celebrations == CelebrationEnum.Big && 10 <= level;

            // Check if enough resources to start a celebration
            if (!MiscCost.EnoughResForCelebration(Vill, bigCeleb))
            {
                ResourcesHelper.NotEnoughRes(acc, Vill, MiscCost.CelebrationCost(bigCeleb), this);
                return TaskRes.Executed;
            }

            await StartCelebration(acc, bigCeleb);

            // Post task check for celebration duration
            Vill.Expansion.CelebrationEnd = TimeParser.GetCelebrationTime(acc.Wb.Html);

            if (Vill.Expansion.Celebrations != CelebrationEnum.None) NextExecute = Vill.Expansion.CelebrationEnd;

            return TaskRes.Executed;
        }

        private async Task StartCelebration(Account acc, bool big)
        {
            var nodes = acc.Wb.Html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));

            var node = big ? nodes.LastOrDefault() : nodes.FirstOrDefault();
            if (node == null) return;

            var button = node.Descendants("button").FirstOrDefault();

            await DriverHelper.ExecuteScript(acc, $"document.getElementById('{button.Id}').click()");
        }
    }
}