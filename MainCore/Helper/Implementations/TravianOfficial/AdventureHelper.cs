using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace MainCore.Helper.Implementations.TravianOfficial
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

            _result = _generalHelper.Click(By.XPath(finishButton.XPath));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}