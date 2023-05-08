using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class ClickHelper : Base.ClickHelper
    {
        public ClickHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory) : base(villageCurrentlyBuildingParser, chromeManager, heroSectionParser, generalHelper, contextFactory)
        {
        }

        public override Result ClickStartAdventure(int accountId, int x, int y)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _heroSectionParser.GetStartAdventureButton(html, x, y);
            if (finishButton is null)
            {
                return Result.Fail(new Retry("Cannot find start adventure button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find start adventure button"));
            }

            {
                var result = _generalHelper.Click(accountId, finishElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        public override Result ClickStartFarm(int accountId, int farmId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var farmNode = html.GetElementbyId($"raidList{farmId}");
            if (farmNode is null)
            {
                return Result.Fail(new Retry("Cannot found farm node"));
            }
            var startNode = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("startButton"));
            if (startNode is null)
            {
                return Result.Fail(new Retry("Cannot found start button"));
            }
            var startElements = chromeBrowser.GetChrome().FindElements(By.XPath(startNode.XPath));
            if (startElements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot found start button"));
            }
            {
                var result = _generalHelper.Click(accountId, startElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }
    }
}