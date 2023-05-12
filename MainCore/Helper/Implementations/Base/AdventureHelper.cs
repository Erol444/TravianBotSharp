using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class AdventureHelper : IAdventureHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IChromeManager _chromeManager;
        protected readonly IGeneralHelper _generalHelper;
        protected readonly IUpdateHelper _updateHelper;

        protected readonly IHeroSectionParser _heroSectionParser;
        protected readonly ISystemPageParser _systemPageParser;

        protected Result _result;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        protected Adventure _adventure;

        public AdventureHelper(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IGeneralHelper generalHelper, IHeroSectionParser heroSectionParser, ISystemPageParser systemPageParser)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _generalHelper = generalHelper;
            _heroSectionParser = heroSectionParser;
            _systemPageParser = systemPageParser;
        }

        public void Load(int accountId, CancellationToken cancellationToken)
        {
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
            _generalHelper.Load(-1, accountId, cancellationToken);
            _updateHelper.Load(-1, accountId, cancellationToken);
        }

        public abstract Result ToAdventure();

        public Result StartAdventure()
        {
            _result = ChooseAdventures();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = ClickStartAdventure();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Result ChooseAdventures()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(_accountId);
            if (!setting.IsAutoAdventure)
            {
                return Result.Fail(new Skip("Auto send adventures is set off."));
            }
            var hero = context.Heroes.Find(_accountId);
            if (hero.Status != Enums.HeroStatusEnums.Home)
            {
                return Result.Fail(new Skip("Hero is not at home."));
            }

            var adventures = context.Adventures.Where(a => a.AccountId == _accountId);
            _adventure = adventures.FirstOrDefault();
            if (_adventure is null) return Result.Fail(new Skip("No adventure available"));
            return Result.Ok();
        }

        protected abstract Result ClickStartAdventure();

        public DateTime GetAdventureTimeLength()
        {
            var html = _chromeBrowser.GetHtml();
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