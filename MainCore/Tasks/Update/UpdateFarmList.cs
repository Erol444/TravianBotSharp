using MainCore.Enums;
using MainCore.Helper;
using System;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateFarmList : BotTask
    {
        public UpdateFarmList(int accountId) : base(accountId)
        {
        }

        public override string Name => "Update farmlist";

        public override void Execute()
        {
            var village = GetVillageHasRallyPoint();
            if (Cts.IsCancellationRequested) return;
            if (village == -1)
            {
                _logManager.Warning(AccountId, "There is no rallypoint in your villages");
                return;
            }
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.SwitchVillage(context, _chromeBrowser, village, AccountId);
                if (Cts.IsCancellationRequested) return;
            }
            {
                var result = GotoFarmListPage(village);
                if (Cts.IsCancellationRequested) return;
                if (!result) return;
            }
            {
                using var context = _contextFactory.CreateDbContext();
                UpdateHelper.UpdateFarmList(context, _chromeBrowser, AccountId);
                _eventManager.OnFarmListUpdated(AccountId);
            }
        }

        private int GetVillageHasRallyPoint()
        {
            using var context = _contextFactory.CreateDbContext();

            var currentVillage = CheckHelper.GetCurrentVillageId(_chromeBrowser);
            if (currentVillage != -1)
            {
                var building = context.VillagesBuildings
                   .Where(x => x.VillageId == currentVillage)
                   .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
                if (building is not null) return currentVillage;
            }

            var villages = context.Villages.Where(x => x.AccountId == AccountId);

            foreach (var village in villages)
            {
                if (Cts.IsCancellationRequested) return -1;
                var building = context.VillagesBuildings
                    .Where(x => x.VillageId == village.Id)
                    .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
                if (building is null) continue;
                return village.Id;
            }
            return -1;
        }

        private bool GotoFarmListPage(int village)
        {
            if (!CheckHelper.IsFarmListPage(_chromeBrowser))
            {
                using var context = _contextFactory.CreateDbContext();
                var url = _chromeBrowser.GetCurrentUrl();
                if (!url.Contains("dorf2"))
                {
                    NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId, true);
                    UpdateHelper.UpdateCurrentlyBuilding(context, _chromeBrowser, village);
                    UpdateHelper.UpdateDorf2(context, _chromeBrowser, AccountId, village);
                }
                else
                {
                    var updateTime = context.VillagesUpdateTime.Find(village);
                    if (updateTime.Dorf2 - DateTime.Now > TimeSpan.FromMinutes(1))
                    {
                        NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId, true);
                        UpdateHelper.UpdateCurrentlyBuilding(context, _chromeBrowser, village);
                        UpdateHelper.UpdateDorf2(context, _chromeBrowser, AccountId, village);
                    }
                }
                if (Cts.IsCancellationRequested) return false;

                NavigateHelper.GoToBuilding(_chromeBrowser, 39, context, AccountId);
                if (Cts.IsCancellationRequested) return false;
                NavigateHelper.SwitchTab(_chromeBrowser, 4, context, AccountId);
            }

            return true;
        }
    }
}