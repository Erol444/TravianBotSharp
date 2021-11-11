using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Tasks.SecondLevel
{
    public class SendRoute : SendResource
    {
        public SendRoute(Village vill, DateTime executeAt, TaskPriority priority = TaskPriority.Medium) : base(vill, null, null, executeAt, priority)
        {
        }

        public override async Task<TaskRes> Execute(Account acc)
        {
            var index = Vill.Market.TradeRoute.Next;
            var route = Vill.Market.TradeRoute.TradeRoutes[index];
            while (!route.Active)
            {
                index = MarketHelper.UpdateNextTradeRoute(Vill);
                route = Vill.Market.TradeRoute.TradeRoutes[index];
            }

            Coordinates = route.Location;
            Resources = route.Resource;
            await base.Execute(acc);
            if (Duration != null)
                MarketHelper.UpdateNextTradeRoute(Vill);

            return TaskRes.Executed;
        }
    }
}