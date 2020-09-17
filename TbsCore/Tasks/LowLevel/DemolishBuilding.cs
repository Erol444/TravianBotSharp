using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
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
            var htmlDoc = acc.Wb.Html;
            var wb = acc.Wb.Driver;

            var mainBuilding = vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.MainBuilding);
            if (mainBuilding == null) return TaskRes.Executed;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={mainBuilding.Id}");

            if (vill.Build.DemolishTasks.Count == 0) return TaskRes.Executed; //No more demolish tasks

            var id = BuildingToDemolish(vill, htmlDoc);

            if (id == null) return TaskRes.Executed; //No more demolish tasks

            this.PostTaskCheck.Add(CheckDemolishTime);

            wb.ExecuteScript($"document.getElementById('demolish').value={id}");
            await Task.Delay(AccountHelper.Delay());
            wb.ExecuteScript($"document.getElementById('btn_demolish').click()");
            this.NextExecute = DateTime.Now.AddMinutes(10);
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
            //TODO: get name of the building you are destroying. localization.
            var option = building.InnerText;
            var lvl = option.Split(' ').LastOrDefault();
            var buildingName = Parser.RemoveNumeric(option.Split('.')[1]).Trim();
            var optionBuilding = Localizations.BuildingFromString(buildingName);
            if (int.Parse(lvl) <= task.Level || optionBuilding != task.Building)
            {
                vill.Build.DemolishTasks.Remove(task);
                return BuildingToDemolish(vill, htmlDoc);
            }
            return task.BuildingId;
        }

        /// <summary>
        /// PostCheckTask. Add new demolish task after this one finishes.
        /// </summary>
        /// <param name="htmlDoc">The html of the page</param>
        /// <param name="acc">account</param>
        public void CheckDemolishTime(HtmlDocument htmlDoc, Account acc)
        {
            var table = htmlDoc.GetElementbyId("demolish");
            if (table == null) //No building is being demolished
            {
                this.NextExecute = DateTime.Now;
                return;
            }
            //Re-execute the demolish building task
            this.NextExecute = DateTime.Now.Add(TimeParser.ParseTimer(table)).AddSeconds(2);
        }
    }
}