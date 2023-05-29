using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class AdventureHelper : Base.AdventureHelper
    {
        public AdventureHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IHeroSectionParser heroSectionParser, ISystemPageParser systemPageParser, IDatabaseHelper databaseHelper) : base(chromeManager, generalHelper, heroSectionParser, systemPageParser, databaseHelper)
        {
        }

        public override Result ToAdventure(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetAdventuresButton(html);
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find adventures button"));
            }
            var result = _generalHelper.Click(By.XPath(node.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Wait(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var adventureDiv = doc.GetElementbyId("heroAdventure");
                if (adventureDiv is null) return false;
                var heroState = adventureDiv.Descendants("div").FirstOrDefault(x => x.HasClass("heroState"));
                if (heroState is null) return false;
                return driver.FindElements(By.XPath(heroState.XPath)).Count > 0;
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _updateHelper.UpdateAdventures();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public override Result ClickStartAdventure(int accountId, Adventure adventure)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _heroSectionParser.GetStartAdventureButton(html, adventure.X, adventure.Y);
            if (finishButton is null)
            {
                return Result.Fail(new Retry("Cannot find start adventure button"));
            }

            var result = _generalHelper.Click(By.XPath(finishButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}