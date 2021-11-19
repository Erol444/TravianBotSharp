using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Tasks.SecondLevel
{
    public class SendToMain : SendResource
    {
        public SendToMain(Village vill, Village villMain, DateTime executeAt, TaskPriority priority = TaskPriority.Medium) : base(vill, MarketHelper.Round(MarketHelper.GetResource(vill, vill.Market.AutoMarket.SendToMain.Amount)), villMain.Coordinates, executeAt, priority)
        {
        }

        public override async Task<TaskRes> Execute(Account acc)
        {
            if (!Vill.Market.AutoMarket.SendToMain.Enabled) return TaskRes.Executed;
            await base.Execute(acc);
            return TaskRes.Executed;
        }
    }
}