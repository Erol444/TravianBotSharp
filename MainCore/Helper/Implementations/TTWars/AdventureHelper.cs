using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;

namespace MainCore.Helper.Implementations.TTWars
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

            var result = _generalHelper.Click(accountId, By.XPath(node.XPath));
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

            var result = _generalHelper.Click(accountId, By.XPath(finishButton.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Wait(accountId, driver =>
            {
                var elements = driver.FindElements(By.Id("start"));
                if (elements.Count == 0) return false;
                return elements[0].Enabled && elements[0].Displayed;
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Click(accountId, By.Id("start"), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Wait(accountId, driver =>
            {
                var elements = driver.FindElements(By.Id("ok"));
                if (elements.Count == 0) return false;
                return elements[0].Enabled && elements[0].Displayed;
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}