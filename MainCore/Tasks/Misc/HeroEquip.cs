using MainCore.Enums;
using MainCore.Helper;
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

        public override string Name => $"Hero equip {VillageId}";
        private readonly List<(HeroItemEnums, int)> _items;

        private readonly int _villageId;
        public int VillageId => _villageId;

        public override void Execute()
        {
            NavigateHelper.ToHeroInventory(ChromeBrowser);
            using var context = ContextFactory.CreateDbContext();
            if (VillageId != -1) NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
            var heroStatus = context.Heroes.Find(AccountId).Status;

            foreach ((var item, var amount) in _items)
            {
                if (heroStatus != HeroStatusEnums.Home && !item.IsUsableWhenHeroAway())
                {
                    return;
                }

                HeroHelper.ClickItem(ChromeBrowser, item);
                if (amount == 1)
                {
                    continue;
                }
                else
                {
                    HeroHelper.EnterAmount(ChromeBrowser, amount);
                    HeroHelper.Confirm(ChromeBrowser);
                }
                Thread.Sleep(600);
            }
        }
    }
}