using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;

namespace MainCore.Helper.Implementations.Base
{
    public class CompleteNowHelper : ICompleteNowHelper
    {
        private readonly IChromeManager _chromeManager;

        private readonly IVillageCurrentlyBuildingParser _villageCurrentlyBuildingParser;

        private readonly IGeneralHelper _generalHelper;

        public CompleteNowHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IGeneralHelper generalHelper, IChromeManager chromeManager)
        {
            _villageCurrentlyBuildingParser = villageCurrentlyBuildingParser;
            _generalHelper = generalHelper;
            _chromeManager = chromeManager;
        }

        public Result Execute(int accountId, int villageId)
        {
            var result = _generalHelper.ToDorf(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = ClickCompleteNowButton(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = ClickConfirmCompleteNowButton(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.ToDorf(accountId, villageId, true);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Result ClickCompleteNowButton(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetFinishButton(html);
            if (finishButton is null)
            {
                return Result.Fail(Retry.ButtonNotFound("complete now"));
            }
            var result = _generalHelper.Click(accountId, By.XPath(finishButton.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Wait(accountId, driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var confirmButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
                return confirmButton is not null;
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Result ClickConfirmCompleteNowButton(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
            if (finishButton is null)
            {
                return Result.Fail(Retry.ButtonNotFound("confirm"));
            }

            var result = _generalHelper.Click(accountId, By.XPath(finishButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}