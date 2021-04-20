using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.ResourceModels;
using TravBotSharp.Files.Helpers;

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
            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace, "&t=5"))
                return TaskRes.Executed;

            if (Resources == null) Resources = Vill.Res.Stored.Resources;
            // Check if we have enough resources in main village
            var resToSend = MarketHelper.SendResCapToStorage(acc, Resources);

            var targetVillage = acc.Villages.FirstOrDefault(x => x.Coordinates == Coordinates);

            var duration = await MarketHelper.MarketSendResource(acc, resToSend, targetVillage, this);

            var targetVill = acc.Villages.FirstOrDefault(x => x.Coordinates == Coordinates);
            targetVill.Market.Settings.Configuration.TransitArrival = DateTime.Now.Add(duration);

            if (Configuration != null && duration != null) Configuration.TransitArrival = DateTime.Now.Add(duration);
            // When you send resources there actually isn't a page load
            return TaskRes.Executed;
        }
    }
}