using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class RallypointHelper : Base.RallypointHelper
    {
        public RallypointHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, IUpdateHelper updateHelper) : base(chromeManager, generalHelper, contextFactory, updateHelper)
        {
        }

        protected override Result ClickStartFarm(int farmId)
        {
            if (!_generalHelper.IsPageValid()) return Result.Fail(Stop.Announcement);
            var html = _chromeBrowser.GetHtml();

            var farmNode = html.GetElementbyId($"raidList{farmId}");
            if (farmNode is null)
            {
                return Result.Fail(new Retry("Cannot found farm node"));
            }
            var startNode = farmNode.Descendants("button")
                                    .FirstOrDefault(x => x.HasClass("startButton"));
            if (startNode is null)
            {
                return Result.Fail(new Retry("Cannot found start button"));
            }

            _result = _generalHelper.Click(By.XPath(startNode.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}