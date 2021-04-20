using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SellOnAuctions : BotTask
    {
        public int ItemId { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await VersionHelper.Navigate(acc, "/hero.php?t=4&action=sell", "/hero/auction?action=sell");

            acc.Wb.Driver.ExecuteScript("document.getElementsByClassName(\"green ok dialogButtonOk\")[0].click()");

            return TaskRes.Executed;
        }
    }
}