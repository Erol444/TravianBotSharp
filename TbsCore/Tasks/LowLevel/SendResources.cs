

using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Tasks.ResourcesConfiguration;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendResources : BotTask
    {
        public SendResourcesConfiguration Configuration { get; set; }
        public Resources Resources { get; set; }
        public Coordinates Coordinates { get; set; }
        public int RunTimes { get; set; } //once / twice / 3 times

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Marketplace);
            if (building == null)
            {
                //update dorg, no buildingId found?
                TaskExecutor.AddTask(acc, new UpdateDorf2() { ExecuteAt = DateTime.Now, Vill = Vill });
                Console.WriteLine($"There is no {building} in this village!");
                return TaskRes.Executed;
            }
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={building.Id}&t=5");

            if (this.Resources == null)
            {
                this.Resources = Vill.Res.Stored.Resources;
            }
            // Check if we have enough resources in main village
            var resToSend = MarketHelper.SendResCapToStorage(acc, this.Resources);

            var targetVillage = acc.Villages.FirstOrDefault(x => x.Coordinates == this.Coordinates);
            var duration = await MarketHelper.MarketSendResource(acc, resToSend, targetVillage, this);

            var targetVill = acc.Villages.FirstOrDefault(x => x.Coordinates == Coordinates);
            targetVill.Market.Settings.Configuration.TransitArrival = DateTime.Now.Add(duration);

            if (this.Configuration != null && duration != null)
            {
                this.Configuration.TransitArrival = DateTime.Now.Add(duration);
                this.Configuration.LastTransit = DateTime.Now;
            }
            // When you send resources there actually isn't a page load
            return TaskRes.Executed;
        }
    }
}
