using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
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

            (var itemCategory, var itemName, var itemTier) = HeroHelper.ParseHeroItem(Item);
            if (itemCategory != HeroItemCategory.Others)
            {
                // Check if hero is at home
                if (acc.Hero.Status != Hero.StatusEnum.Home)
                {
                    // Wait for hero to come home
                    var nextExecute = acc.Hero.NextHeroSend > acc.Hero.HeroArrival ?
                        acc.Hero.NextHeroSend :
                        acc.Hero.HeroArrival;

                    this.NextExecute = nextExecute;
                    return TaskRes.Executed;
                }
            }

            string script = "var items = document.getElementById('itemsToSale');";

            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.T4_5:
                    script += $"items.querySelector('div[class$=\"_{(int)Item}\"]').click();";
                    break;
                case ServerVersionEnum.T4_4:
                    script += $"items.querySelector('div[class$=\"_{(int)Item} \"]').click();";
                    break;

            }

            await DriverHelper.ExecuteScript(acc, script);

            // No amount specified, meaning we have already equipt the item
            if (Amount == -1) return Done(acc);

            try
            {
                script = $"document.getElementById('amount').value = {Amount};";
                acc.Wb.Driver.ExecuteScript(script);
            }
            catch (Exception e) {
                // When using book / artwork / bucket, you don't specify amount, but you have to confirm usage
            }

            script = "document.querySelector('div[class=\"buttons\"]>button').click();";
            await DriverHelper.ExecuteScript(acc, script);

            return Done(acc);
        }

        /// <summary>
        /// Refresh the hero page after equipping an item
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TaskRes</returns>

        private TaskRes Done(Account acc)
        {
            HeroHelper.ParseHeroPage(acc);
            return TaskRes.Executed;
        }
    }
}
