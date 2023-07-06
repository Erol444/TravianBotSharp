using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.TTWars
{
    public class RallypointHelper : Base.RallypointHelper
    {
        public RallypointHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, IUpdateHelper updateHelper) : base(chromeManager, generalHelper, contextFactory, updateHelper)
        {
        }

        public override Result ClickStartFarm(int accountId, int farmId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.GetChrome();
            chrome.ExecuteScript($"Travian.Game.RaidList.toggleList({farmId});");
            _generalHelper.DelayClick(accountId);

            var result = _generalHelper.Wait(accountId, driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);

                var waitFarmNode = waitHtml.GetElementbyId($"list{farmId}");
                var table = waitFarmNode.Descendants("div").FirstOrDefault(x => x.HasClass("listContent"));
                return !table.HasClass("hide");
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Click(accountId, By.Id($"raidListMarkAll{farmId}"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var html = chromeBrowser.GetHtml();
            var farmNode = html.GetElementbyId($"list{farmId}");
            var buttonStartFarm = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("green") && x.GetAttributeValue("type", "").Contains("submit"));

            if (buttonStartFarm is null)
            {
                return Result.Fail(new Retry("Cannot find button start farmlist"));
            }

            result = _generalHelper.Click(accountId, By.XPath(buttonStartFarm.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}