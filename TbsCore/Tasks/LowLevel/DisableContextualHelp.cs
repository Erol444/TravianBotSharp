using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class DisableContextualHelp : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/options");
            await DriverHelper.ClickById(acc, "v13");

            var acceptButton = acc.Wb.Html.DocumentNode
                .Descendants("div")
                .First(x => x.HasClass("submitButtonContainer"))
                .Descendants("button")
                .First();

            await DriverHelper.ClickById(acc, acceptButton.Id);

            return TaskRes.Executed;
        }
    }
}
