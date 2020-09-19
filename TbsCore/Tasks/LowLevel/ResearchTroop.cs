
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Parsers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ResearchTroop : BotTask
    {
        //If Troop == null, just update the troop levels
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            var academy = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Academy);
            if (academy == null)
            {
                return TaskRes.Executed;
            }
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={academy.Id}");

            var troop = Vill.Troops.ToResearch.FirstOrDefault();
            if (troop == TroopsEnum.None) return TaskRes.Executed; //We have researched all troops that were on the list

            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
            if (troopNode == null)
            {
                this.ErrorMessage = $"Researching {troop} was not possible! Bot assumes you already have it researched";
                Vill.Troops.Researched.Add(troop);
                return TaskRes.Retry;
            }
            while (!troopNode.HasClass("research")) troopNode = troopNode.ParentNode;

            var button = troopNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
            if (button == null)
            {
                RepeatTask(Vill, troop, DateTime.Now);
            }
            (TimeSpan dur, Resources cost) = TroopsParser.AcademyResearchCost(acc.Wb.Html, troop);

            var nextExecute = ResourcesHelper.EnoughResourcesOrTransit(acc, Vill, cost);

            if (nextExecute < DateTime.Now.AddMilliseconds(1)) //We have enough resources, click Research button
            {
                wb.ExecuteScript($"document.getElementById('{button.Id}').click()");
                var executeNext = DateTime.Now.Add(dur).AddMilliseconds(10 * AccountHelper.Delay());
                TaskExecutor.AddTask(acc,
                    new ImproveTroop() { Vill = this.Vill, ExecuteAt = DateTime.Now.Add(dur) }
                    );
                RepeatTask(Vill, troop, executeNext);

                return TaskRes.Executed;
            }
            else //Retry same task after resources get produced/transited
            {
                this.NextExecute = nextExecute;
                return TaskRes.Executed;
            }
        }

        private void RepeatTask(Village vill, Classificator.TroopsEnum troop, DateTime nextExecute)
        {
            vill.Troops.ToResearch.Remove(troop);
            //Next research when this one finishes
            if (vill.Troops.ToResearch.Count > 0) this.NextExecute = nextExecute;
        }
    }
}
