using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class UseHeroResources : VillageBotTask
    {
        private readonly IHeroResourcesHelper _heroHelper;

        public UseHeroResources(int villageId, int accountId, List<(HeroItemEnums, int)> items, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _heroHelper = Locator.Current.GetService<IHeroResourcesHelper>();
            _items = items;
        }

        private readonly List<(HeroItemEnums, int)> _items;

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                SwitchVillage,
                UseItem,
            };

            foreach (var command in commands)
            {
                _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                var result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            return Result.Ok();
        }

        private Result SwitchVillage()
        {
            if (VillageId != -1) return _generalHelper.SwitchVillage(AccountId, VillageId);
            return Result.Ok();
        }

        private Result UseItem()
        {
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