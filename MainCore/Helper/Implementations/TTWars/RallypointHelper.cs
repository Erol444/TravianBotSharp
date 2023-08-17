using FluentResults;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TTWars
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
            return Result.Ok();
        }
    }
}