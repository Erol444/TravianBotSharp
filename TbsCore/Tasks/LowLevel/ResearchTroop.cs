using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ResearchTroop : UpdateDorf2
    {
        //If Troop == null, just update the troop levels
        public override async Task<TaskRes> Execute(Account acc)
        {
            await base.Execute(acc); // Navigate to dorf2

            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Academy))
                return TaskRes.Executed;

            var troop = Vill.Troops.ToResearch.FirstOrDefault();
            if (troop == TroopsEnum.None) return TaskRes.Executed; //We have researched all troops that were on the list

            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
            if (troopNode == null)
            {
                acc.Wb.Log($"Researching {troop} was not possible! Bot assumes you already have it researched");
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

            // Check if we have enough resources to research the troop
            if (!ResourcesHelper.IsEnoughRes(Vill, cost.ToArray()))
            {
                ResourcesHelper.NotEnoughRes(acc, Vill, cost, this);
                return TaskRes.Executed;
            }

            await DriverHelper.ClickById(acc, button.Id);
            
            var executeNext = DateTime.Now.Add(dur).AddMilliseconds(10 * AccountHelper.Delay());
            if (Vill.Settings.AutoImprove)
            {
                TaskExecutor.AddTask(acc,new ImproveTroop() { Vill = this.Vill, ExecuteAt = DateTime.Now.Add(dur) });
            }
            
            RepeatTask(Vill, troop, executeNext);

            return TaskRes.Executed;
        }

        private void RepeatTask(Village vill, Classificator.TroopsEnum troop, DateTime nextExecute)
        {
            vill.Troops.ToResearch.Remove(troop);
            //Next research when this one finishes
            if (0 < vill.Troops.ToResearch.Count) this.NextExecute = nextExecute;
        }
    }
}
