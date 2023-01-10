using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class UseHeroResources : VillageBotTask
    {
        private readonly IHeroHelper _heroHelper;

        public UseHeroResources(int villageId, int accountId, List<(HeroItemEnums, int)> items, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _heroHelper = Locator.Current.GetService<IHeroHelper>();
            _items = items;
        }

        private readonly List<(HeroItemEnums, int)> _items;

        public override Result Execute()
        {
            if (VillageId != -1) _navigateHelper.SwitchVillage(AccountId, VillageId);
            using var context = _contextFactory.CreateDbContext();
            var heroStatus = context.Heroes.Find(AccountId).Status;
            var setting = context.AccountsSettings.Find(AccountId);
            foreach ((var item, var amount) in _items)
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    return Result.Fail(new Cancel());
                }
                if (heroStatus != HeroStatusEnums.Home && !item.IsUsableWhenHeroAway())
                {
                    return Result.Ok();
                }
                if (amount < 0) continue;
                _heroHelper.ClickItem(AccountId, item);

                if (amount <= 1)
                {
                    continue;
                }
                else
                {
                    Thread.Sleep(Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax));
                    _heroHelper.EnterAmount(AccountId, RoundUpTo100(amount));
                    _heroHelper.Confirm(AccountId);
                    Thread.Sleep(Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax));
                }
                Thread.Sleep(Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax));
            }
            return Result.Ok();
        }

        private static int RoundUpTo100(int res)
        {
            var remainder = res % 100;
            return res + (100 - remainder);
        }
    }
}