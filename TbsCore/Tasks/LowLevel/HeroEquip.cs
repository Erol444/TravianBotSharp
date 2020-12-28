using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class HeroEquip : BotTask
    {
        public List<(HeroItemEnum, int)> Items { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php");

            HeroHelper.ParseHeroPage(acc);

            foreach (var use in Items)
            {
                var (item, amount) = use;

                var (category, name, tier) = HeroHelper.ParseHeroItem(item);
                if (category != HeroItemCategory.Others)
                {
                    // Check if hero is at home
                    if (acc.Hero.Status != Hero.StatusEnum.Home)
                    {
                        // Wait for hero to come home
                        var nextExecute = acc.Hero.NextHeroSend > acc.Hero.HeroArrival ?
                            acc.Hero.NextHeroSend :
                            acc.Hero.HeroArrival;

                        var in5Min = DateTime.Now.AddMinutes(5);
                        if (nextExecute < in5Min) nextExecute = in5Min;
                        this.NextExecute = nextExecute;
                        return TaskRes.Retry;
                    }
                }

                string script = "var items = document.getElementById('itemsToSale');";

                switch (acc.AccInfo.ServerVersion)
                {
                    case ServerVersionEnum.T4_5:
                        script += $"items.querySelector('div[class$=\"_{(int)item}\"]').click();";
                        break;
                    case ServerVersionEnum.T4_4:
                        script += $"items.querySelector('div[class$=\"_{(int)item} \"]').click();";
                        break;

                }

                await DriverHelper.ExecuteScript(acc, script);

                // No amount specified, meaning we have already equipt the item
                if (amount == 0) return Done(acc);

                try
                {
                    script = $"document.getElementById('amount').value = {amount};";
                    acc.Wb.Driver.ExecuteScript(script);
                }
                catch (Exception e)
                {
                    // When using book / artwork / bucket, you don't specify amount, but you have to confirm usage
                }

                script = "document.querySelector('div[class=\"buttons\"]>button').click();";
                await DriverHelper.ExecuteScript(acc, script);
            }

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
