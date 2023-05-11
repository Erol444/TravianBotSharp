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

        protected override Result ClickStartFarm(int farmId)
        {
            if (!_generalHelper.IsPageValid()) return Result.Fail(Stop.Announcement);

            var chrome = _chromeBrowser.GetChrome();
            chrome.ExecuteScript($"Travian.Game.RaidList.toggleList({farmId});");
            _generalHelper.DelayClick();

            _result = _generalHelper.Wait(driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);

                var waitFarmNode = waitHtml.GetElementbyId($"list{farmId}");
                var table = waitFarmNode.Descendants("div").FirstOrDefault(x => x.HasClass("listContent"));
                return !table.HasClass("hide");
            });
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Click(By.Id($"raidListMarkAll{farmId}"));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            var html = _chromeBrowser.GetHtml();
            var farmNode = html.GetElementbyId($"list{farmId}");
            var buttonStartFarm = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("green") && x.GetAttributeValue("type", "").Contains("submit"));

            if (buttonStartFarm is null)
            {
                return Result.Fail(new Retry("Cannot find button start farmlist"));
            }

            _result = _generalHelper.Click(By.XPath(buttonStartFarm.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}