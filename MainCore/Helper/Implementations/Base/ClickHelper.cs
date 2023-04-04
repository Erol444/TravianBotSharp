using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parser.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class ClickHelper : IClickHelper
    {
        protected readonly IChromeManager _chromeManager;
        protected readonly IVillageCurrentlyBuildingParser _villageCurrentlyBuildingParser;
        protected readonly IHeroSectionParser _heroSectionParser;
        protected readonly INavigateHelper _navigateHelper;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ClickHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IHeroSectionParser heroSectionParser, INavigateHelper navigateHelper, IDbContextFactory<AppDbContext> contextFactory)
        {
            _villageCurrentlyBuildingParser = villageCurrentlyBuildingParser;
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
            _navigateHelper = navigateHelper;
            _contextFactory = contextFactory;
        }

        public Result ClickCompleteNow(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            {
                var result = ClickCompleteNowButton(accountId, chromeBrowser);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            try
            {
                WaitDialogCompleteNow(chromeBrowser);
            }
            catch
            {
                return Result.Fail(new Retry("Cannot find diaglog complete now"));
            }
            {
                var result = ClickConfirmFinishNowButton(accountId, chromeBrowser);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result ClickCompleteNowButton(int accountId, IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetFinishButton(html);
            if (finishButton is null)
            {
                return Result.Fail(new Retry("Cannot find complete now button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find complete now button"));
            }
            {
                var result = _navigateHelper.Click(accountId, finishElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private void WaitDialogCompleteNow(IChromeBrowser chromeBrowser)
        {
            var wait = chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var confirmButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
                return confirmButton is not null;
            });
        }

        private Result ClickConfirmFinishNowButton(int accountId, IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
            if (finishButton is null)
            {
                return Result.Fail(new Retry("Cannot find confirm button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find confirm button"));
            }
            {
                var result = _navigateHelper.Click(accountId, finishElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public abstract Result ClickStartAdventure(int accountId, int x, int y);

        public abstract Result ClickStartFarm(int accountId, int farmId);
    }
}