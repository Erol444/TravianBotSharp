using MainCore.Helper;
using MainCore.Tasks.Sim;
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
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            _name = $"Update village in {village.Name}";
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
            using var context = ContextFactory.CreateDbContext();
            NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
        }

        private void Update()
        {
            using var context = ContextFactory.CreateDbContext();
            var currentUrl = ChromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf"))
            {
                UpdateHelper.UpdateCurrentlyBuilding(context, ChromeBrowser, VillageId);
                InstantUpgrade();
            }
            if (currentUrl.Contains("dorf1"))
            {
                UpdateHelper.UpdateDorf1(context, ChromeBrowser, VillageId);
                UpdateHelper.UpdateProduction(context, ChromeBrowser, VillageId);
            }
            else if (currentUrl.Contains("dorf2"))
            {
                UpdateHelper.UpdateDorf2(context, ChromeBrowser, AccountId, VillageId);
            }

            UpdateHelper.UpdateResource(context, ChromeBrowser, VillageId);
        }

        private void InstantUpgrade()
        {
            using var context = ContextFactory.CreateDbContext();

            var setting = context.VillagesSettings.Find(VillageId);
            if (!setting.IsInstantComplete) return;
            var info = context.AccountsInfo.Find(AccountId);
            if (info.Gold < 2) return;
            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Level != -1);
            if (currentlyBuilding.Count() < (info.HasPlusAccount ? 2 : 1)) return;
            if (currentlyBuilding.Last().CompleteTime < DateTime.Now.AddMinutes(setting.InstantCompleteTime)) return;
            var listTask = TaskManager.GetList(AccountId);
            var tasks = listTask.Where(x => x.GetType() == typeof(InstantUpgrade)).OfType<InstantUpgrade>().Where(x => x.VillageId == VillageId);
            if (tasks.Any()) return;
            TaskManager.Add(AccountId, new InstantUpgrade(VillageId, AccountId));
        }
    }
}