using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ImproveTroop : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
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
            TroopsHelper.UpdateResearchedTroops(Vill);

            var currentlyImproving = TroopsParser.GetImprovingTroops(acc.Wb.Html);
            var troop = TroopToImprove(Vill, currentlyImproving);
            if (troop == Classificator.TroopsEnum.None) return TaskRes.Executed;

            //If we have plus account we can improve 2 troops at the same time
            var maxImproving = acc.AccInfo.PlusAccount ? 2 : 1;
            if (maxImproving <= currentlyImproving.Count())
            {
                NextExecute = DateTime.Now.Add(currentlyImproving.Last().Time);
                return TaskRes.Executed;
            }
            //call NextImprove() after enough res OR when this improvement finishes.

            var cost = Vill.Troops.Levels.FirstOrDefault(x => x.Troop == troop);

            // Check if we have enough resources to improve the troop
            if (!ResourcesHelper.IsEnoughRes(Vill, cost.UpgradeCost.ToArray()))
            {
                ResourcesHelper.NotEnoughRes(acc, Vill, cost.UpgradeCost, this);
                return TaskRes.Executed;
            }

            //Click on the button
            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img")
                .FirstOrDefault(x => x.HasClass("u" + (int) troop));
            while (!troopNode.HasClass("research")) troopNode = troopNode.ParentNode;

            var button = troopNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
            if (button == null)
            {
                acc.Wb.Log($"Could not find Upgrade button to improve {troop}");
                NextExecute = DateTime.Now.AddMinutes(1);
                return TaskRes.Retry;
            }

            await DriverHelper.ClickById(acc, button.Id);

            // If we have plus account and there is currently no other troop to improve, go ahead and improve the unit again
            NextExecute = currentlyImproving.Count() == 0 && maxImproving == 2
                ? DateTime.MinValue
                : DateTime.Now.Add(cost.TimeCost).AddMilliseconds(5 * AccountHelper.Delay());
            return TaskRes.Executed;
        }

        private Classificator.TroopsEnum TroopToImprove(Village vill, List<TroopCurrentlyImproving> improving)
        {
            var troop = vill.Troops.ToImprove.FirstOrDefault();
            if (troop == Classificator.TroopsEnum.None) return troop;
            //how many times is this troop improving (0/1/2)
            var alreadyImprovingTime = improving.Count(x => x.Troop == troop);

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