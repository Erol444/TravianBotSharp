using MainCore.Helper;
using MainCore.Tasks.Sim;
using System;
using System.Linq;
using MainCore.Tasks.Misc;

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

using MainCore.Enums;

#elif TTWARS

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : VillageBotTask
    {
        public UpdateVillage(int villageId, int accountId) : base(villageId, accountId, "Update village")
        {
        }

        public UpdateVillage(int villageId, int accountId, string name) : base(villageId, accountId, name)
        {
        }

        public override void Execute()
        {
            Navigate();
            UpdateAccountInfo();
            UpdateVillageInfo();
        }

        private void Navigate()
        {
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
        }

        private void UpdateAccountInfo()
        {
            var updateTask = new UpdateInfo(AccountId);
            updateTask.CopyFrom(this);
            updateTask.Execute();
        }

        private void UpdateVillageInfo()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf"))
            {
                UpdateHelper.UpdateCurrentlyBuilding(context, _chromeBrowser, VillageId);
                InstantUpgrade(context);
                _eventManager.OnVillageCurrentUpdate(VillageId);
            }
            if (currentUrl.Contains("dorf1"))
            {
                UpdateHelper.UpdateDorf1(context, _chromeBrowser, VillageId);
                _eventManager.OnVillageBuildsUpdate(VillageId);

                UpdateHelper.UpdateProduction(context, _chromeBrowser, VillageId);
            }
            else if (currentUrl.Contains("dorf2"))
            {
                UpdateHelper.UpdateDorf2(context, _chromeBrowser, AccountId, VillageId);
                _eventManager.OnVillageBuildsUpdate(VillageId);
            }

            UpdateHelper.UpdateResource(context, _chromeBrowser, VillageId);
            AutoNPC(context);
        }

        private void InstantUpgrade(AppDbContext context)
        {
            var setting = context.VillagesSettings.Find(VillageId);
            if (!setting.IsInstantComplete) return;
            var info = context.AccountsInfo.Find(AccountId);
            if (info.Gold < 2) return;
            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Level != -1);
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            var tribe = context.AccountsInfo.Find(AccountId).Tribe;
            if (tribe == TribeEnums.Romans)
            {
                if (currentlyBuilding.Count(x => x.Level != -1) < (info.HasPlusAccount ? 3 : 2)) return;
            }
            else
            {
                if (currentlyBuilding.Count(x => x.Level != -1) < (info.HasPlusAccount ? 2 : 1)) return;
            }
#elif TTWARS
            if (currentlyBuilding.Count(x => x.Level != -1) < (info.HasPlusAccount ? 2 : 1)) return;
#else

#error You forgot to define Travian version here

#endif
            if (currentlyBuilding.Max(x => x.CompleteTime) < DateTime.Now.AddMinutes(setting.InstantCompleteTime)) return;
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<InstantUpgrade>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;
            _taskManager.Add(AccountId, new InstantUpgrade(VillageId, AccountId));
        }

        private void AutoNPC(AppDbContext context)
        {
            var info = context.AccountsInfo.Find(AccountId);
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            if (info.Gold < 3) return;
#elif TTWARS
            if (info.Gold < 5) return;

#else

#error You forgot to define Travian version here

#endif

            var setting = context.VillagesSettings.Find(VillageId);
            if (!setting.IsAutoNPC) return;
            if (setting.AutoNPCPercent == 0) return;

            var resource = context.VillagesResources.Find(VillageId);
            var ratio = resource.Crop * 100.0f / resource.Granary;
            if (ratio > setting.AutoNPCPercent) return;

            var npcTasks = _taskManager.GetList(AccountId).OfType<NPCTask>();
            if (npcTasks.Any(x => x.VillageId == VillageId)) return;

            _taskManager.Add(AccountId, new NPCTask(VillageId, AccountId));
        }
    }
}