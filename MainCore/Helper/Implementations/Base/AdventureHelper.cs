using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using System;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class AdventureHelper : IAdventureHelper
    {
        protected readonly IDatabaseHelper _databaseHelper;
        protected readonly IChromeManager _chromeManager;
        protected readonly IGeneralHelper _generalHelper;
        protected readonly IUpdateHelper _updateHelper;

        protected readonly ISystemPageParser _systemPageParser;
        protected readonly IHeroSectionParser _heroSectionParser;

        protected AdventureHelper(IDatabaseHelper databaseHelper, IChromeManager chromeManager, IGeneralHelper generalHelper, IUpdateHelper updateHelper, ISystemPageParser systemPageParser, IHeroSectionParser heroSectionParser)
        {
            _databaseHelper = databaseHelper;
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
            _updateHelper = updateHelper;
            _systemPageParser = systemPageParser;
            _heroSectionParser = heroSectionParser;
        }

        public abstract Result ToAdventure(int accountId);

        public abstract Result ClickStartAdventure(int accountId, Adventure adventure);

        public Result StartAdventure(int accountId)
        {
            var result = ChooseAdventures(accountId);
            if (result.IsFailed) return Result.Fail(result.Errors).WithError(new Trace(Trace.TraceMessage()));
            var adventure = result.Value;
            result = ClickStartAdventure(accountId, adventure);
            if (result.IsFailed) return Result.Fail(result.Errors).WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Result<Adventure> ChooseAdventures(int accountId)
        {
            var setting = _databaseHelper.GetAccountSetting(accountId);
            if (!setting.IsAutoAdventure)
            {
                return Result.Fail(new Skip("Auto send adventures is set off."));
            }
            var hero = _databaseHelper.GetHero(accountId);
            if (hero.Status != Enums.HeroStatusEnums.Home)
            {
                return Result.Fail(new Skip("Hero is not at home."));
            }

            var adventures = _databaseHelper.GetAdventures(accountId);
            var adventure = adventures.FirstOrDefault();
            if (adventure is null) return Result.Fail(new Skip("No adventure available"));
            return Result.Ok(adventure);
        }

        public DateTime GetAdventureTimeLength(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);

            var html = chromeBrowser.GetHtml();
            var tileDetails = _systemPageParser.GetAdventuresDetail(html);
            if (tileDetails is null)
            {
                return DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
            }
            var timer = tileDetails.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            if (timer is null)
            {
                return DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
            }

            int sec = int.Parse(timer.GetAttributeValue("value", "0"));
            if (sec < 0) sec = 0;
            return DateTime.Now.AddMinutes(sec * 2 + Random.Shared.Next(5, 10));
        }
    }
}