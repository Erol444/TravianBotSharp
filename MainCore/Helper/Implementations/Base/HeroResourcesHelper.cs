using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class HeroResourcesHelper : IHeroResourcesHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IChromeManager _chromeManager;
        protected readonly IHeroSectionParser _heroSectionParser;
        protected readonly IGeneralHelper _generalHelper;

        public HeroResourcesHelper(IChromeManager chromeManager, IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory)
        {
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
        }

        public Result Execute(int accountId, int villageId, HeroItemEnums item, int amount)
        {
            var result = _generalHelper.SwitchVillage(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = ClickItem(accountId, item);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = EnterAmount(accountId, amount);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public abstract Result ClickItem(int accountId, HeroItemEnums item);

        public abstract Result Confirm(int accountId);

        public Result EnterAmount(int accountId, int amount)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var amountBox = _heroSectionParser.GetAmountBox(doc);
            if (amountBox is null)
            {
                return Result.Fail(new Retry("Cannot find amount box"));
            }

            var result = _generalHelper.Input(accountId, By.XPath(amountBox.XPath), $"{RoundUpTo100(amount)}");
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private static int RoundUpTo100(int res)
        {
            var remainder = res % 100;
            return res + (100 - remainder);
        }

        public Result FillResource(int accountId, int villageId, Resources cost)
        {
            using var context = _contextFactory.CreateDbContext();

            var itemsHero = context.HeroesItems.Where(x => x.AccountId == accountId);
            var woodAvaliable = itemsHero.FirstOrDefault(x => x.Item == HeroItemEnums.Wood);
            var clayAvaliable = itemsHero.FirstOrDefault(x => x.Item == HeroItemEnums.Clay);
            var ironAvaliable = itemsHero.FirstOrDefault(x => x.Item == HeroItemEnums.Iron);
            var cropAvaliable = itemsHero.FirstOrDefault(x => x.Item == HeroItemEnums.Crop);

            var resAvaliable = new Resources(woodAvaliable?.Count ?? 0, clayAvaliable?.Count ?? 0, ironAvaliable?.Count ?? 0, cropAvaliable?.Count ?? 0);
            var resLeft = resAvaliable - cost;
            if (resLeft.IsNegative())
            {
                return Result.Fail(NoResource.Hero(cost));
            }

            var items = new List<(HeroItemEnums, int)>()
                            {
                                (HeroItemEnums.Wood, (int)cost.Wood),
                                (HeroItemEnums.Clay, (int)cost.Clay),
                                (HeroItemEnums.Iron, (int)cost.Iron),
                                (HeroItemEnums.Crop, (int)cost.Crop),
                            };

            foreach (var item in items)
            {
                var result = Execute(accountId, villageId, item.Item1, item.Item2);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}