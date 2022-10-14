using MainCore.Enums;
using MainCore.Helper;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateFarmList : AccountBotTask
    {
        public UpdateFarmList(int accountId) : base(accountId, "Update farmlist")
        {
        }

        public UpdateFarmList(int accountId, string name) : base(accountId, name)
        {
        }

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

                var taskUpdate = new UpdateDorf2(village, AccountId);
                taskUpdate.CopyFrom(this);
                taskUpdate.Execute();

                if (Cts.IsCancellationRequested) return false;

                NavigateHelper.GoToBuilding(_chromeBrowser, 39, context, AccountId);
                if (Cts.IsCancellationRequested) return false;
                NavigateHelper.SwitchTab(_chromeBrowser, 4, context, AccountId);
            }
            return true;
        }
    }
}