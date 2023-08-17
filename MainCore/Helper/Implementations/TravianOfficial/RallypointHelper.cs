using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class RallypointHelper : Base.RallypointHelper
    {
        public RallypointHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, IUpdateHelper updateHelper, IFarmListParser farmListParser) : base(chromeManager, generalHelper, contextFactory, updateHelper, farmListParser)
        {
        }

        public override Result ClickStartFarm(int accountId, int farmId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.GetChrome();
            chrome.ExecuteScript($"Travian.Game.RaidList.startRaid({farmId});");
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

            return Result.Ok();
        }
    }
}