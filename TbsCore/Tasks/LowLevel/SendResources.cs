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


            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace, "&t=5"))
                return TaskRes.Executed;

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
