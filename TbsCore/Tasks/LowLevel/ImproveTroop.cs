using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.TroopsModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ImproveTroop : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            if (Vill == null) Vill = acc.Villages.First(x => x.Active);

            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Smithy))
                return TaskRes.Executed;

            var levels = TroopsParser.GetTroopLevels(acc.Wb.Html);
            if (levels == null)
            {
                acc.Wb.Log("There was an error at getting Smithy troop levels");
                return TaskRes.Executed;
            }
            Vill.Troops.Levels = levels;
            UpdateResearchedTroops(Vill);

            var currentlyImproving = TroopsParser.GetImprovingTroops(acc.Wb.Html);
            var troop = TroopToImprove(Vill, currentlyImproving);
            if (troop == Classificator.TroopsEnum.None)
            {
                return TaskRes.Executed;
            }

            //If we have plus account we can improve 2 troops at the same time
            int maxImproving = acc.AccInfo.PlusAccount ? 2 : 1;
            if (currentlyImproving.Count() >= maxImproving)
            {
                this.NextExecute = DateTime.Now.Add(currentlyImproving.Last().Time);
                return TaskRes.Executed;
            }
            //call NextImprove() after enough res OR when this improvement finishes.

            var cost = Vill.Troops.Levels.FirstOrDefault(x => x.Troop == troop);

            var nextExecute = ResourcesHelper.EnoughResourcesOrTransit(acc, Vill, cost.UpgradeCost);
            if (nextExecute < DateTime.Now.AddMilliseconds(1)) //We have enough resources, click Improve button
            {
                //Click on the button
                var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
                while (!troopNode.HasClass("research")) troopNode = troopNode.ParentNode;

                var button = troopNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
                if (button == null)
                {
                    acc.Wb.Log($"Could not find Upgrade button to improve {troop}");
                    this.NextExecute = DateTime.Now.AddMinutes(1);
                    return TaskRes.Retry;
                }


                wb.FindElementById(button.Id).Click();
                // If we have plus account and there is currently no other troop to improve, go ahead and improve the unit again
                this.NextExecute = (currentlyImproving.Count() == 0 && maxImproving == 2) ?
                    DateTime.MinValue :
                    DateTime.Now.Add(cost.TimeCost).AddMilliseconds(5 * AccountHelper.Delay());
                return TaskRes.Executed;

            }
            else //Retry same task after resources get produced/transited
            {
                this.NextExecute = nextExecute;
                return TaskRes.Executed;
            }
        }

        private void UpdateResearchedTroops(Village vill)
        {
            if (vill.Troops.Levels.Count > 0) vill.Troops.Researched = vill.Troops.Levels.Select(x => x.Troop).ToList();
        }

        private Classificator.TroopsEnum TroopToImprove(Village vill, List<TroopCurrentlyImproving> improving)
        {
            var troop = vill.Troops.ToImprove.FirstOrDefault();
            if (troop == Classificator.TroopsEnum.None) return troop;
            //how many times is this troop improving (0/1/2)
            int alreadyImprovingTime = improving.Count(x => x.Troop == troop);

            if (!vill.Troops.Levels.Exists(x => x.Troop == troop) ||
                vill.Troops.Levels.First(x => x.Troop == troop).Level + alreadyImprovingTime >= 20)
            {
                vill.Troops.ToImprove.Remove(troop);
                return TroopToImprove(vill, improving);
            }
            return troop;
        }
    }
}
