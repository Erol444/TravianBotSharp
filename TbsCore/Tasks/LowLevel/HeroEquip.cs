using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class HeroEquip : BotTask
    {
        public HeroItemEnum Item { get; set; }
        public int Amount { get; set; } = -1;
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php");

            HeroHelper.ParseHeroPage(acc);

            string script = "var items = document.getElementById('itemsToSale');";
            script += $"items.querySelector('div[class$=\"_{(int)Item}\"]').click();";

            await DriverHelper.ExecuteScript(acc, script);

            // No amount specified, meaning we have already equipt the item
            if (Amount == -1) return TaskRes.Executed;

            script = $"document.getElementById('amount').value = {Amount};";
            acc.Wb.Driver.ExecuteScript(script);

            script = "document.querySelector('div[class=\"buttons\"]>button').click();";
            acc.Wb.Driver.ExecuteScript(script);

            return TaskRes.Executed;
        }
    }
}
