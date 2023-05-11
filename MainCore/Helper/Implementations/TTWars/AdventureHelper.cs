using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace MainCore.Helper.Implementations.TTWars
{
    public class AdventureHelper : Base.AdventureHelper
    {
        public AdventureHelper(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IGeneralHelper generalHelper, IHeroSectionParser heroSectionParser) : base(chromeManager, contextFactory, generalHelper, heroSectionParser)
        {
        }

        protected override Result ClickStartAdventure()
        {
            if (!_generalHelper.IsPageValid()) return Result.Fail(Stop.Announcement);

            var html = _chromeBrowser.GetHtml();
            var finishButton = _heroSectionParser.GetStartAdventureButton(html, _adventure.X, _adventure.Y);
            if (finishButton is null)
            {
                return Result.Fail(new Retry("Cannot find start adventure button"));
            }

            _result = _generalHelper.Click(By.XPath(finishButton.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Wait(driver =>
            {
                var elements = driver.FindElements(By.Id("start"));
                if (elements.Count == 0) return false;
                return elements[0].Enabled && elements[0].Displayed;
            });
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Click(By.Id("start"), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Wait(driver =>
            {
                var elements = driver.FindElements(By.Id("ok"));
                if (elements.Count == 0) return false;
                return elements[0].Enabled && elements[0].Displayed;
            });
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}