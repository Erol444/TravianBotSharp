using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.Settings;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Send all resources above X% to main village.
    /// TODO: If we have auto celebration selected, leave res for that (calculate based on production)
    /// </summary>
    public class SendResToMain : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            // If this is the main village, don't try to send resources
            if (AccountHelper.GetMainVillage(acc) == this.Vill) return TaskRes.Executed;

            var res = MarketHelper.GetResToMainVillage(Vill);
            if (res.Sum() <= 0) return TaskRes.Executed;

            if (!await NavigationHelper.ToMarketplace(acc, Vill, NavigationHelper.MarketplaceTab.SendResources))
                return TaskRes.Executed;

            if (this.Vill.Settings.Type == VillType.Support && this.Vill.Settings.SendRes)
            {
                // Repeat this task
                var ran = new Random();
                this.NextExecute = DateTime.Now.AddMinutes(ran.Next(30, 60));
            }

            var mainVill = AccountHelper.GetMainVillage(acc);
            var ret = await MarketHelper.MarketSendResource(acc, res, mainVill, this);
            return TaskRes.Executed;
        }
    }
}