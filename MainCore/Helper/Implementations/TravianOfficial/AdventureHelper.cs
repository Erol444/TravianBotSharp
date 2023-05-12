using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class AdventureHelper : Base.AdventureHelper
    {
        public AdventureHelper(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IGeneralHelper generalHelper, IHeroSectionParser heroSectionParser) : base(chromeManager, contextFactory, generalHelper, heroSectionParser)
        {
        }

        public override Result ToAdventure()
        {
            if (!_generalHelper.IsPageValid()) return Result.Fail(Stop.Announcement);

            var html = _chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetAdventuresButton(html);
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find adventures button"));
            }
            _result = _generalHelper.Click(By.XPath(node.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Wait(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var adventureDiv = doc.GetElementbyId("heroAdventure");
                if (adventureDiv is null) return false;
                var heroState = adventureDiv.Descendants("div").FirstOrDefault(x => x.HasClass("heroState"));
                if (heroState is null) return false;
                return driver.FindElements(By.XPath(heroState.XPath)).Count > 0;
            });
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _updateHelper.UpdateAdventures();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
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