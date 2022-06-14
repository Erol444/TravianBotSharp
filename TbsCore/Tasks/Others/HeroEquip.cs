using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.Others
{
    public class HeroEquip : BotTask
    {
        public List<(HeroItemEnum, int)> Items { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.ToHero(acc, NavigationHelper.HeroTab.Inventory);

            acc.Hero.Items = HeroParser.GetHeroInventory(acc.Wb.Html);
            acc.Hero.Equipt = HeroParser.GetHeroEquipment(acc.Wb.Html);

            foreach (var use in Items)
            {
                var (item, amount) = use;

                var (category, _, _) = HeroHelper.ParseHeroItem(item);
                if (category != HeroItemCategory.Resource && category != HeroItemCategory.Stackable && category != HeroItemCategory.NonStackable)
                {
                    // Check if hero is at home
                    if (acc.Hero.Status != Hero.StatusEnum.Home)
                    {
                        acc.Logger.Warning("Hero isn't in home village");
                        // Wait for hero to come home
                        var nextExecute = acc.Hero.NextHeroSend > acc.Hero.HeroArrival ?
                            acc.Hero.NextHeroSend :
                            acc.Hero.HeroArrival;

                        var in5Min = DateTime.Now.AddMinutes(5);
                        if (nextExecute < in5Min) nextExecute = in5Min;
                        NextExecute = nextExecute;
                        return TaskRes.Retry;
                    }
                }

                var itemNode = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass($"item{(int)item}"));
                itemNode = itemNode.ParentNode;
                var elements = acc.Wb.Driver.FindElements(By.XPath(itemNode.XPath));
                elements[0].Click();
                var wait = new WebDriverWait(acc.Wb.Driver, TimeSpan.FromMinutes(1));
                wait.Until(driver => driver.FindElements(By.Id("consumableHeroItem")).Count > 0);
                acc.Wb.UpdateHtml();

                var form = acc.Wb.Html.GetElementbyId("consumableHeroItem");
                var input = form.Descendants("input").FirstOrDefault();
                var inputElements = acc.Wb.Driver.FindElements(By.XPath(input.XPath));

                inputElements[0].SendKeys(Keys.Home);
                inputElements[0].SendKeys(Keys.Shift + Keys.End);
                inputElements[0].SendKeys($"{amount}");

                var dialog = acc.Wb.Html.GetElementbyId("dialogContent");
                var buttonWrapper = dialog.Descendants("div").FirstOrDefault(x => x.HasClass("buttonsWrapper"));
                var buttonTransfer = buttonWrapper.Descendants("button").ToArray();
                var buttonTransferElements = acc.Wb.Driver.FindElements(By.XPath(buttonTransfer[1].XPath));
                buttonTransferElements[0].Click();

                wait.Until(driver =>
                {
                    acc.Wb.UpdateHtml();
                    var inventoryPageWrapper = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                    return !inventoryPageWrapper.HasClass("loading");
                });
            }

            acc.Hero.Items = HeroParser.GetHeroInventory(acc.Wb.Html);
            acc.Hero.Equipt = HeroParser.GetHeroEquipment(acc.Wb.Html);

            return TaskRes.Executed;
        }
    }
}