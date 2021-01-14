using System;
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
            var townHall = Vill.Build
                .Buildings
                .FirstOrDefault(x => x.Type == Classificator.BuildingEnum.TownHall);

            if (townHall == null) return TaskRes.Executed;

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={townHall.Id}");

            var celebrationEnd = TimeParser.GetCelebrationTime(acc.Wb.Html);
            if (DateTime.Now <= celebrationEnd)
            {
                // We already have a celebration running
                this.NextExecute = celebrationEnd;
                return TaskRes.Executed;
            }

            var bigCeleb = Vill.Expansion.Celebrations == CelebrationEnum.Big && 10 <= townHall.Level;

            // Check if enough resources
            if (!MiscCost.EnoughResForCelebration(Vill, bigCeleb))
            {
                // If we don't have enough res, wait until enough res / transit
                this.NextExecute = ResourcesHelper.EnoughResourcesOrTransit(acc, Vill, MiscCost.CelebrationCost(bigCeleb));
                return TaskRes.Executed;
            }
            await StartCelebration(acc, bigCeleb);

            // Post task check for celebration duration
            Vill.Expansion.CelebrationEnd = TimeParser.GetCelebrationTime(acc.Wb.Html);

            if (Vill.Expansion.Celebrations != CelebrationEnum.None) this.NextExecute = Vill.Expansion.CelebrationEnd;

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
