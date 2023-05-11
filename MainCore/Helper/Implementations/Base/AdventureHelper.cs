using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class AdventureHelper : IAdventureHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IChromeManager _chromeManager;
        protected readonly IGeneralHelper _generalHelper;

        protected readonly IHeroSectionParser _heroSectionParser;

        protected Result _result;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        protected Adventure _adventure;

        public AdventureHelper(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IGeneralHelper generalHelper, IHeroSectionParser heroSectionParser)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _generalHelper = generalHelper;
            _heroSectionParser = heroSectionParser;
        }

        public void Load(int accountId, CancellationToken cancellationToken)
        {
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
            _generalHelper.Load(-1, accountId, cancellationToken);
        }

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
            var adventures = context.Adventures.Where(a => a.AccountId == _accountId);
            _adventure = adventures.FirstOrDefault();
            if (_adventure is null) return Result.Fail(new Skip("No adventure available"));
            return Result.Ok();
        }

        protected abstract Result ClickStartAdventure();
    }
}