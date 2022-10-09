using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Services;
using OpenQA.Selenium;
using System;
using System.Threading;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.FindElements;

#elif TRAVIAN_OFFICIAL_HEROUI

using System.Linq;
using TravianOfficialNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.FindElements;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Helper
{
    public static class HeroHelper
    {
        public static bool IsUsableWhenHeroAway(this HeroItemEnums item)
        {
            return item switch
            {
                HeroItemEnums.Ointment or HeroItemEnums.Scroll or HeroItemEnums.Bucket or HeroItemEnums.Tablets or HeroItemEnums.Book or HeroItemEnums.Artwork or HeroItemEnums.SmallBandage or HeroItemEnums.BigBandage or HeroItemEnums.Cage or HeroItemEnums.Wood or HeroItemEnums.Clay or HeroItemEnums.Iron or HeroItemEnums.Crop => true,
                _ => false,
            };
        }

        public static void ClickItem(IChromeBrowser chromeBrowser, HeroItemEnums item)
        {
            var doc = chromeBrowser.GetHtml();
            var node = HeroPage.GetItemSlot(doc, (int)item);
            if (node is null)
            {
                throw new Exception($"Cannot find item {item}");
            }

            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));
            if (elements.Count == 0)
            {
                throw new Exception($"Cannot find item {item}");
            }

            elements[0].Click();
            var wait = chromeBrowser.GetWait();
            if (item.IsUsableWhenHeroAway())
            {
                wait.Until(driver =>
                {
                    var html = new HtmlDocument();
                    html.LoadHtml(driver.PageSource);
                    var dialog = HeroPage.GetAmountBox(html);
                    return dialog is not null;
                });
            }
            else
            {
#if TRAVIAN_OFFICIAL_HEROUI
                wait.Until(driver =>
                {
                    var html = new HtmlDocument();
                    html.LoadHtml(driver.PageSource);
                    var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                    return !inventoryPageWrapper.HasClass("loading");
                });
#elif TTWARS || TRAVIAN_OFFICIAL
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
#else

#error You forgot to define Travian version here

#endif
            }
        }

        public static void EnterAmount(IChromeBrowser chromeBrowser, int amount)
        {
            var doc = chromeBrowser.GetHtml();
            var amountBox = HeroPage.GetAmountBox(doc);
            if (amountBox is null)
            {
                throw new Exception("Cannot find amount box");
            }
            var chrome = chromeBrowser.GetChrome();
            var amountInputs = chrome.FindElements(By.XPath(amountBox.XPath));
            if (amountInputs.Count == 0)
            {
                throw new Exception("Cannot find amount box");
            }
            amountInputs[0].SendKeys(Keys.Home);
            amountInputs[0].SendKeys(Keys.Shift + Keys.End);
            amountInputs[0].SendKeys(amount.ToString());
        }

        public static void Confirm(IChromeBrowser chromeBrowser)
        {
            var doc = chromeBrowser.GetHtml();
            var confirmButton = HeroPage.GetConfirmButton(doc);
            if (confirmButton is null)
            {
                throw new Exception("Cannot find confirm button");
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(confirmButton.XPath));
            if (elements.Count == 0)
            {
                throw new Exception("Cannot find confirm button");
            }
            elements[0].Click();
            var wait = chromeBrowser.GetWait();
#if TRAVIAN_OFFICIAL_HEROUI
            wait.Until(driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });
#elif TTWARS || TRAVIAN_OFFICIAL
            Thread.Sleep(3000);
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
#else

#error You forgot to define Travian version here

#endif
        }
    }
}