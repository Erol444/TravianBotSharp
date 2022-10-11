using MainCore.Enums;
using MainCore.Helper;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class UseHeroResources : VillageBotTask
    {
        public UseHeroResources(int villageId, int accountId, List<(HeroItemEnums, int)> items) : base(villageId, accountId, "Use hero's resources")
        {
            _items = items;
        }

        private readonly List<(HeroItemEnums, int)> _items;
        private readonly Random _rand = new();

        public override void Execute()
        {
            using var context = _contextFactory.CreateDbContext();
            if (VillageId != -1) NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
            var heroStatus = context.Heroes.Find(AccountId).Status;
            var setting = context.AccountsSettings.Find(AccountId);
            var wait = _chromeBrowser.GetWait();
            foreach ((var item, var amount) in _items)
            {
                if (Cts.IsCancellationRequested) return;
                if (heroStatus != HeroStatusEnums.Home && !item.IsUsableWhenHeroAway())
                {
                    return;
                }
                if (amount < 0) continue;
                HeroHelper.ClickItem(_chromeBrowser, item);

                if (amount <= 1)
                {
                    continue;
                }
                else
                {
                    Thread.Sleep(_rand.Next(setting.ClickDelayMin, setting.ClickDelayMax));
                    HeroHelper.EnterAmount(_chromeBrowser, RoundUpTo100(amount));
                    HeroHelper.Confirm(_chromeBrowser);
                    Thread.Sleep(_rand.Next(setting.ClickDelayMin, setting.ClickDelayMax));
                }
                Thread.Sleep(_rand.Next(setting.ClickDelayMin, setting.ClickDelayMax));
            }
        }

        private static int RoundUpTo100(int res)
        {
            var remainder = res % 100;
            return res + (100 - remainder);
        }
    }
}