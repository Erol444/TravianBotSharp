using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class TTWarsExpandStorage : BotTask
    {
        public int Seconds { get; set; }
        public int Times { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Level > 0 && (x.Type == Classificator.BuildingEnum.Warehouse || x.Type == Classificator.BuildingEnum.Granary));
            if (building != null) await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={building.Id}");

            //expand the storage
            //TODO change this with GOLD options -> expand storage button (like buy res/animals)
            var button = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("increaseStorage"));
            if (button == null)
            {
                acc.Wb.Log("No such button, are you sure you are on TTWars vip/unl?");
                return TaskRes.Executed;
            }
            wb.FindElementById(button.Id).Click();

            if (this.Times > 1)
            {
                //repeat the task
                this.NextExecute = DateTime.Now.AddSeconds(2);
                this.Times--;
            }
            else if (this.Seconds > 0)
            {
                this.NextExecute = DateTime.Now.AddSeconds(this.Seconds);
            }
            return TaskRes.Executed;
        }
    }
}
