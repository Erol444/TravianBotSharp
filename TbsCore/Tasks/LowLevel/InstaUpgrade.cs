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
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");

            var finishClass = acc.Wb.Html.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("finishNow"));
            var button = finishClass.Descendants("button").FirstOrDefault();
            await TbsCore.Helpers.DriverHelper.ExecuteScript(acc, $"document.getElementById('{button.GetAttributeValue("id", "")}').click()");

            var dialog = acc.Wb.Html.GetElementbyId("finishNowDialog");
            var useButton = dialog.Descendants("button").FirstOrDefault();
            await DriverHelper.ExecuteScript(acc, $"document.getElementById('{useButton.GetAttributeValue("id", "")}').click()");

            // Execute next build task right away
            var task = acc.Tasks.FirstOrDefault(x =>
                x.GetType() == typeof(UpgradeBuilding) &&
                x.Vill == this.Vill
            );
            if (task != null)
            {
                task.ExecuteAt = DateTime.Now;
            }

            await TaskExecutor.PageLoaded(acc);

            return TaskRes.Executed;
        }
    }
}