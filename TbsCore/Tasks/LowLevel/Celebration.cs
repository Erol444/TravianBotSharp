using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TbsCore.Parsers;
using TbsCore.TravianData;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class Celebration : BotTask
    {
        public bool BigCelebration { get; set; }
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

            // Check if enough resources

            var bigCeleb = BigCelebration && 10 <= townHall.Level;

            if (!MiscCost.EnoughResForCelebration(Vill, bigCeleb))
            {
                // If we don't have enough res, wait until enough res / transit
                this.NextExecute = ResourcesHelper.EnoughResourcesOrTransit(acc, Vill, MiscCost.CelebrationCost(bigCeleb));
                return TaskRes.Executed;
            }
            await StartCelebration(acc, bigCeleb);

            // Post task check for celebration duration
            Vill.Expansion.CelebrationEnd = TimeParser.GetCelebrationTime(acc.Wb.Html);

            if (Vill.Expansion.AutoCelebrations) this.NextExecute = Vill.Expansion.CelebrationEnd;

            return TaskRes.Executed;
        }

        private async Task StartCelebration(Account acc, bool big)
        {
            var nodes = acc.Wb.Html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));

            var node = big ? nodes.LastOrDefault() : nodes.FirstOrDefault();
            if (node == null) return;

            var button = node.Descendants("button").FirstOrDefault();

            await acc.Wb.Driver.FindElementById(button.Id.ToString()).Click(acc);
        }
    }
}
