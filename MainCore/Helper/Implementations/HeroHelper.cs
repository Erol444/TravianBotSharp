using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using ModuleCore.Parser;
using OpenQA.Selenium;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations
{
    public class HeroHelper : IHeroHelper
    {
        private readonly IChromeManager _chromeManager;
        private readonly IHeroSectionParser _heroSectionParser;
        private readonly INavigateHelper _navigateHelper;

        public HeroHelper(IChromeManager chromeManager, IHeroSectionParser heroSectionParser, INavigateHelper navigateHelper)
        {
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
            _navigateHelper = navigateHelper;
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
                    var result = _navigateHelper.WaitPageLoaded(accountId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
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

                var result = _navigateHelper.WaitPageLoaded(accountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}