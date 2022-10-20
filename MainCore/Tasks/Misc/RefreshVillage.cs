﻿using MainCore.Tasks.Update;
using System;

namespace MainCore.Tasks.Misc
{
    public class RefreshVillage : VillageBotTask
    {
        private readonly Random rand = new();

        public RefreshVillage(int villageId, int accountId) : base(villageId, accountId, "Refresh village")
        {
        }

        public override void Execute()
        {
            BotTask taskUpdate;
            if (IsNeedDorf2())
            {
                taskUpdate = new UpdateBothDorf(VillageId, AccountId);
            }
            else
            {
                taskUpdate = new UpdateDorf1(VillageId, AccountId);
            }
            taskUpdate.CopyFrom(this);
            taskUpdate.Execute();

            NextExecute();
        }

        private bool IsNeedDorf2()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            return setting.IsUpgradeTroop;
        }

        private void NextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            var delay = rand.Next(setting.AutoRefreshTimeMin, setting.AutoRefreshTimeMax);
            ExecuteAt = DateTime.Now.AddMinutes(delay);
        }
    }
}