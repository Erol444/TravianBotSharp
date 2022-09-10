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
                LogManager.Warning(AccountId, "There is no rallypoint in your villages");
                return;
            }
            {
                using var context = ContextFactory.CreateDbContext();
                NavigateHelper.SwitchVillage(context, ChromeBrowser, village);
                if (Cts.IsCancellationRequested) return;
            }
            {
                var result = GotoFarmListPage(village);
                if (Cts.IsCancellationRequested) return;
                if (!result) return;
            }
            {
                using var context = ContextFactory.CreateDbContext();
                UpdateHelper.UpdateFarmList(context, ChromeBrowser, AccountId);
            }
        }

        private int GetVillageHasRallyPoint()
        {
            using var context = ContextFactory.CreateDbContext();

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
            using var context = ContextFactory.CreateDbContext();
            var url = ChromeBrowser.GetCurrentUrl();
            if (!url.Contains("dorf2"))
            {
                NavigateHelper.ToDorf2(ChromeBrowser, true);
            }
            else
            {
                var updateTime = context.VillagesUpdateTime.Find(village);
                if (updateTime.Dorf2 - DateTime.Now > TimeSpan.FromMinutes(1))
                {
                    NavigateHelper.ToDorf2(ChromeBrowser, true);
                }
            }
            if (Cts.IsCancellationRequested) return false;

            UpdateHelper.UpdateCurrentlyBuilding(context, ChromeBrowser, village);
            UpdateHelper.UpdateDorf2(context, ChromeBrowser, AccountId, village);
            var location = context.VillagesBuildings.Where(x => x.VillageId == village).FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint).Id;
            NavigateHelper.GoToBuilding(ChromeBrowser, location);
            if (Cts.IsCancellationRequested) return false;

            NavigateHelper.SwitchTab(ChromeBrowser, 4);
            return true;
        }
    }
}