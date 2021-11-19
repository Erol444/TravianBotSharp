using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.LowLevel
{
    public class DemolishBuilding : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            // First navigate to dorf2 and then to the main building, to make sure the currently demolish list is refreshed
            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.MainBuilding))
                return TaskRes.Executed;

            if (Vill.Build.DemolishTasks.Count == 0) return TaskRes.Executed; //No more demolish tasks

            var id = BuildingToDemolish(Vill, acc.Wb.Html);

            if (id == null) return TaskRes.Executed; //No more demolish tasks

            await DriverHelper.WriteById(acc, "demolish", id);
            await DriverHelper.ClickById(acc, "btn_demolish");

            this.NextExecute = TimeHelper.RanDelay(acc, await NextDemolishTime(acc));

            return TaskRes.Executed;
        }

        private int? BuildingToDemolish(Village vill, HtmlDocument htmlDoc)
        {
            if (vill.Build.DemolishTasks.Count == 0) return null;

            var task = vill.Build.DemolishTasks.FirstOrDefault();

            var building = htmlDoc.GetElementbyId("demolish").Descendants("option")
                .FirstOrDefault(x =>
                    x.GetAttributeValue("value", "") == task.BuildingId.ToString()
                );

            //If this building doesn't exist or is below/on the correct level, find next building to demolish
            if (building == null)
            {
                vill.Build.DemolishTasks.Remove(task);
                return BuildingToDemolish(vill, htmlDoc);
            }

            var option = building.InnerText;
            var lvl = option.Split(' ').LastOrDefault();

            //TODO: Check if localized building name match
            //var buildingName = Parser.RemoveNumeric(option.Split('.')[1]).Trim();
            //var optionBuilding = Localizations.BuildingFromString(buildingName);

            if (int.Parse(lvl) <= task.Level /*|| optionBuilding != task.Building*/)
            {
                vill.Build.DemolishTasks.Remove(task);
                return BuildingToDemolish(vill, htmlDoc);
            }
            return task.BuildingId;
        }

        /// <summary>
        /// Checks demolish time.
        /// </summary>
        /// <param name="acc">account</param>
        public async Task<DateTime> NextDemolishTime(Account acc)
        {
            // Is this needed?
            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.MainBuilding))
                return DateTime.Now;

            var table = acc.Wb.Html.GetElementbyId("demolish");

            if (table == null) //No building is being demolished
            {
                return DateTime.Now;
            }
            //Re-execute the demolish building task

            var time = DateTime.Now.Add(TimeParser.ParseTimer(table));
            return time.AddSeconds(2);
        }
    }
}