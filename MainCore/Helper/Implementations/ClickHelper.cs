using FluentResults;
using HtmlAgilityPack;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;
using ServerModuleCore.Parser;

namespace MainCore.Helper.Implementations
{
    public class ClickHelper : IClickHelper
    {
        private readonly IChromeManager _chromeManager;
        private readonly IVillageCurrentlyBuildingParser _villageCurrentlyBuildingParser;
        private readonly IHeroSectionParser _heroSectionParser;

        public ClickHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IHeroSectionParser heroSectionParser)
        {
            _villageCurrentlyBuildingParser = villageCurrentlyBuildingParser;
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
        }

        public Result ClickCompleteNow(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetFinishButton(html);
            if (finishButton is null)
            {
                return Result.Fail("Cannot find finish button");
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail("Cannot find finish button");
            }
            finishElements[0].Click();
            return Result.Ok();
        }

        public void WaitDialogFinishNow(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var wait = chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var confirmButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
                return confirmButton is not null;
            });
        }

        public Result ClickConfirmFinishNow(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
            if (finishButton is null)
            {
                return Result.Fail("Cannot find confirm button");
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail("Cannot find confirm button");
            }
            finishElements[0].Click();
            return Result.Ok();
        }

        public Result ClickStartAdventure(int accountId, int x, int y)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _heroSectionParser.GetStartAdventureButton(html, x, y);
            if (finishButton is null)
            {
                return Result.Fail("Cannot find start adventure button");
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail("Cannot find start adventure button");
            }

            finishElements[0].Click();
            if (VersionDetector.IsTTWars())
            {
                var wait = chromeBrowser.GetWait();
                wait.Until(driver =>
                {
                    var elements = driver.FindElements(By.Id("start"));
                    if (elements.Count == 0) return false;
                    return elements[0].Enabled && elements[0].Displayed;
                });

                var elements = chrome.FindElements(By.Id("start"));
                elements[0].Click();

                wait.Until(driver =>
                {
                    var elements = driver.FindElements(By.Id("ok"));
                    if (elements.Count == 0) return false;
                    return elements[0].Enabled && elements[0].Displayed;
                });
            }
            return Result.Ok();
        }
    }
}