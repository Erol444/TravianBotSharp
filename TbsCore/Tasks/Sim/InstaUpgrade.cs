using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Instantly upgrade currently building
    /// </summary>
    public class InstaUpgrade : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.ToDorf1(acc);

            var finishClass = acc.Wb.Html.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("finishNow"));
            var button = finishClass.Descendants("button").FirstOrDefault();
            await DriverHelper.ClickById(acc, button.Id);

            var dialog = acc.Wb.Html.GetElementbyId("finishNowDialog");
            var useButton = dialog.Descendants("button").FirstOrDefault();
            await DriverHelper.ClickById(acc, useButton.Id);

            // Execute next build task right away
            var task = acc.Tasks.FindTask(typeof(UpgradeBuilding), Vill);

            if (task != null)
            {
                task.ExecuteAt = DateTime.Now;
                acc.Tasks.ReOrder();
            }

            await DriverHelper.WaitPageLoaded(acc);

            return TaskRes.Executed;
        }
    }
}