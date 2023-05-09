using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class HeroResourcesHelper : IHeroResourcesHelper
    {
        protected readonly IChromeManager _chromeManager;
        protected readonly IHeroSectionParser _heroSectionParser;
        protected readonly IGeneralHelper _generalHelper;

        protected Result _result;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public HeroResourcesHelper(IChromeManager chromeManager, IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper)
        {
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
            _generalHelper = generalHelper;
        }

        public void Load(int accountId, CancellationToken cancellationToken)
        {
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);

            _generalHelper.Load(-1, accountId, cancellationToken);
        }

        public Result Execute(HeroItemEnums item, int amount)
        {
            _result = ClickItem(item);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            _result = EnterAmount(amount);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        protected abstract Result ClickItem(HeroItemEnums item);

        protected Result EnterAmount(int amount)
        {
            var doc = _chromeBrowser.GetHtml();
            var amountBox = _heroSectionParser.GetAmountBox(doc);
            if (amountBox is null)
            {
                return Result.Fail(new Retry("Cannot find amount box"));
            }

            _result = _generalHelper.Input(By.XPath(amountBox.XPath), $"{amount}");
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        protected abstract Result Confirm();
    }
}