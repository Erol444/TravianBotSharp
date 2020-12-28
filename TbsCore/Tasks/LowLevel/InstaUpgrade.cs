using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
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
            await acc.Wb.Driver.FindElementById(button.Id).Click(acc);

            var dialog = acc.Wb.Html.GetElementbyId("finishNowDialog");
            var useButton = dialog.Descendants("button").FirstOrDefault();

            await acc.Wb.Driver.FindElementById(useButton.Id).Click(acc);

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
