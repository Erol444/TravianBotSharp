using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public class CompleteNowHelper : ICompleteNowHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IChromeManager _chromeManager;

        protected readonly IVillageCurrentlyBuildingParser _villageCurrentlyBuildingParser;

        protected readonly IGeneralHelper _generalHelper;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public CompleteNowHelper(IDbContextFactory<AppDbContext> contextFactory, IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IGeneralHelper generalHelper)
        {
            _contextFactory = contextFactory;
            _villageCurrentlyBuildingParser = villageCurrentlyBuildingParser;
            _generalHelper = generalHelper;
        }

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
            _generalHelper.Load(villageId, accountId, cancellationToken);
        }

        public Result Execute()
        {
            _result = ClickCompleteNowButton();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            try
            {
                var wait = _chromeBrowser.GetWait();
                wait.Until(driver =>
                {
                    var html = new HtmlDocument();
                    html.LoadHtml(driver.PageSource);
                    var confirmButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
                    return confirmButton is not null;
                });
            }
            catch
            {
                return Result.Fail(new Retry("Cannot find diaglog complete now"));
            }

            _result = ClickConfirmCompleteNowButton();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private Result ClickCompleteNowButton()
        {
            var html = _chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetFinishButton(html);
            if (finishButton is null)
            {
                return Result.Fail(Retry.ButtonNotFound("complete now"));
            }
            _result = _generalHelper.Click(By.XPath(finishButton.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private Result ClickConfirmCompleteNowButton()
        {
            var html = _chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
            if (finishButton is null)
            {
                return Result.Fail(Retry.ButtonNotFound("confirm"));
            }

            _result = _generalHelper.Click(By.XPath(finishButton.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}