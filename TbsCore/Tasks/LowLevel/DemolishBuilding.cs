using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.TravianData;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class DemolishBuilding : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;

            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.MainBuilding))
                return TaskRes.Executed;

            if (Vill.Build.DemolishTasks.Count == 0) return TaskRes.Executed; //No more demolish tasks

            var id = BuildingToDemolish(Vill, acc.Wb.Html);

            if (id == null) return TaskRes.Executed; //No more demolish tasks

            await DriverHelper.ExecuteScript(acc, $"document.getElementById('demolish').value={id}");
            //acc.Wb.Driver.FindElementById("demolish")

            await acc.Wb.Driver.FindElementById("btn_demolish").Click(acc);

            this.NextExecute = NextDemolishTime(acc.Wb.Html, acc);

            return TaskRes.Executed;
        }

        private int? BuildingToDemolish(Village vill, HtmlDocument htmlDoc)
        {
            if (vill.Build.DemolishTasks.Count == 0) return null;

            var task = vill.Build.DemolishTasks.FirstOrDefault();

            var building = htmlDoc.GetElementbyId("demolish").ChildNodes
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
        /// <param name="htmlDoc">The html of the page</param>
        /// <param name="acc">account</param>
        public DateTime NextDemolishTime(HtmlDocument htmlDoc, Account acc)
        {
            htmlDoc.LoadHtml(acc.Wb.Driver.PageSource);
            var table = htmlDoc.GetElementbyId("demolish");
            if (table == null) //No building is being demolished
            {
                return DateTime.Now;
            }
            //Re-execute the demolish building task
            return DateTime.Now.Add(TimeParser.ParseTimer(table)).AddSeconds(2);
        }
    }
}