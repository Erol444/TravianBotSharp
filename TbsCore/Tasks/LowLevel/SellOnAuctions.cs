using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    public class SellOnAuctions : BotTask
    {
        public int ItemId { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.ToHero(acc, NavigationHelper.HeroTab.Auctions);

            acc.Wb.ExecuteScript($"document.getElementsByClassName(\"green ok dialogButtonOk\")[0].click()");

            return TaskRes.Executed;
        }
    }
}