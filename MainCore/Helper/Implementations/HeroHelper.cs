using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using ServerModuleCore.Parser;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations
{
    public class HeroHelper : IHeroHelper
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IHeroSectionParser _heroSectionParser;

        public HeroHelper(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IHeroSectionParser heroSectionParser)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
        }

        public Result ClickItem(int accountId, HeroItemEnums item)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetItemSlot(doc, (int)item);
            if (node is null)
            {
                return Result.Fail($"Cannot find item {item}");
            }

            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));
            if (elements.Count == 0)
            {
                return Result.Fail($"Cannot find item {item}");
            }

            elements[0].Click();
            var wait = chromeBrowser.GetWait();
            if (item.IsUsableWhenHeroAway())
            {
                wait.Until(driver =>
                {
                    var html = new HtmlDocument();
                    html.LoadHtml(driver.PageSource);
                    var dialog = _heroSectionParser.GetAmountBox(html);
                    return dialog is not null;
                });
            }
            else
            {
                if (VersionDetector.IsTravianOfficial())
                {
                    wait.Until(driver =>
                    {
                        var html = new HtmlDocument();
                        html.LoadHtml(driver.PageSource);
                        var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                        return !inventoryPageWrapper.HasClass("loading");
                    });
                }
                else if (VersionDetector.IsTTWars())
                {
                    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                }
            }
            return Result.Ok();
        }

        public Result EnterAmount(int accountId, int amount)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var amountBox = _heroSectionParser.GetAmountBox(doc);
            if (amountBox is null)
            {
                return Result.Fail("Cannot find amount box");
            }
            var chrome = chromeBrowser.GetChrome();
            var amountInputs = chrome.FindElements(By.XPath(amountBox.XPath));
            if (amountInputs.Count == 0)
            {
                return Result.Fail("Cannot find amount box");
            }
            amountInputs[0].SendKeys(Keys.Home);
            amountInputs[0].SendKeys(Keys.Shift + Keys.End);
            amountInputs[0].SendKeys(amount.ToString());
            return Result.Ok();
        }

        public Result Confirm(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var confirmButton = _heroSectionParser.GetConfirmButton(doc);
            if (confirmButton is null)
            {
                return Result.Fail("Cannot find confirm button");
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(confirmButton.XPath));
            if (elements.Count == 0)
            {
                return Result.Fail("Cannot find confirm button");
            }
            elements[0].Click();
            var wait = chromeBrowser.GetWait();
            if (VersionDetector.IsTravianOfficial())
            {
                wait.Until(driver =>
                {
                    var html = new HtmlDocument();
                    html.LoadHtml(driver.PageSource);
                    var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                    return !inventoryPageWrapper.HasClass("loading");
                });
            }
            else if (VersionDetector.IsTTWars())
            {
                Thread.Sleep(3000);
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            return Result.Ok();
        }
    }
}