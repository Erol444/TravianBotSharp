using MainCore.Enums;
using MainCore.Helper;
using MainCore.Services;
using MainCore.Tasks.Sim;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : UpdateInfo
    {
        public UpdateVillage(int villageId, int accountId) : base(accountId)
        {
            _villageId = villageId;
        }

        private string _name;
        public override string Name => _name;

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Update village in {VillageId}";
            }
            else
            {
                _name = $"Update village in {village.Name}";
            }
        }

        public override void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IEventManager eventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            base.SetService(contextFactory, chromeBrowser, taskManager, eventManager, logManager, planManager, restClientManager);
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Update village in {VillageId}";
            }
            else
            {
                _name = $"Update village in {village.Name}";
            }
        }

        private readonly int _villageId;
        public int VillageId => _villageId;

        public override void Execute()
        {
            Navigate();
            base.Execute();
            Update();
        }

        private void Navigate()
        {
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
        }

        private void Update()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf"))
            {
                UpdateHelper.UpdateCurrentlyBuilding(context, _chromeBrowser, VillageId);
                InstantUpgrade();
            }
            if (currentUrl.Contains("dorf1"))
            {
                UpdateHelper.UpdateDorf1(context, _chromeBrowser, VillageId);
                UpdateHelper.UpdateProduction(context, _chromeBrowser, VillageId);
            }
            else if (currentUrl.Contains("dorf2"))
            {
                UpdateHelper.UpdateDorf2(context, _chromeBrowser, AccountId, VillageId);
            }

            UpdateHelper.UpdateResource(context, _chromeBrowser, VillageId);
        }

        private void InstantUpgrade()
        {
            using var context = _contextFactory.CreateDbContext();

            var setting = context.VillagesSettings.Find(VillageId);
            if (!setting.IsInstantComplete) return;
            var info = context.AccountsInfo.Find(AccountId);
            if (info.Gold < 2) return;
            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Level != -1);
#if TTWARS
            if (currentlyBuilding.Count(x => x.Level != -1) < (info.HasPlusAccount ? 2 : 1)) return;
#else
            var tribe = context.AccountsInfo.Find(AccountId).Tribe;
            if (tribe == TribeEnums.Romans)
            {
                if (currentlyBuilding.Count(x => x.Level != -1) < (info.HasPlusAccount ? 3 : 2)) return;
            }
            else
            {
                if (currentlyBuilding.Count(x => x.Level != -1) < (info.HasPlusAccount ? 2 : 1)) return;
            }

#endif
            if (currentlyBuilding.Max(x => x.CompleteTime) < DateTime.Now.AddMinutes(setting.InstantCompleteTime)) return;
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.Where(x => x.GetType() == typeof(InstantUpgrade)).OfType<InstantUpgrade>().Where(x => x.VillageId == VillageId);
            if (tasks.Any()) return;
            _taskManager.Add(AccountId, new InstantUpgrade(VillageId, AccountId));
        }
    }
}