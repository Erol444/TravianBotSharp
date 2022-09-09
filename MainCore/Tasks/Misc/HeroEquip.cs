using MainCore.Enums;
using MainCore.Helper;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class HeroEquip : BotTask
    {
        public HeroEquip(int villageId, int accountId, List<(HeroItemEnums, int)> items) : base(accountId)
        {
            _villageId = villageId;
            _items = items;
        }

        private string _name;
        public override string Name => _name;
        private readonly List<(HeroItemEnums, int)> _items;
        private readonly Random _rand = new();

        private readonly int _villageId;
        public int VillageId => _villageId;

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Use resource in {VillageId}";
            }
            else
            {
                _name = $"Use resource in {village.Name}";
            }
        }

        public override void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IEventManager eventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            base.SetService(contextFactory, chromeBrowser, taskManager, eventManager, logManager, planManager, restClientManager);
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Use resource in {VillageId}";
            }
            else
            {
                _name = $"Use resource in {village.Name}";
            }
        }

        public override void Execute()
        {
            using var context = ContextFactory.CreateDbContext();
            if (VillageId != -1) NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
            var heroStatus = context.Heroes.Find(AccountId).Status;
            var setting = context.AccountsSettings.Find(AccountId);
            var wait = ChromeBrowser.GetWait();
            foreach ((var item, var amount) in _items)
            {
                if (Cts.IsCancellationRequested) return;
                if (heroStatus != HeroStatusEnums.Home && !item.IsUsableWhenHeroAway())
                {
                    return;
                }
                if (amount < 0) continue;
                HeroHelper.ClickItem(ChromeBrowser, item);

                if (amount <= 1)
                {
                    continue;
                }
                else
                {
                    Thread.Sleep(_rand.Next(setting.ClickDelayMin, setting.ClickDelayMax));
                    HeroHelper.EnterAmount(ChromeBrowser, RoundUpTo100(amount));
                    HeroHelper.Confirm(ChromeBrowser);
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