using FluentResults;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class HeroHelper : IHeroHelper
    {
        protected readonly IChromeManager _chromeManager;
        protected readonly IHeroSectionParser _heroSectionParser;
        protected readonly IGeneralHelper _generalHelper;

        public HeroHelper(IChromeManager chromeManager, IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper)
        {
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
            _generalHelper = generalHelper;
        }

        public abstract Result ClickItem(int accountId, HeroItemEnums item);

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

        public abstract Result Confirm(int accountId);
    }
}