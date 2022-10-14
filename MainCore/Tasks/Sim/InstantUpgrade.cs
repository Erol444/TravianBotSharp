using MainCore.Helper;
using MainCore.Tasks.Update;
using System;
using System.Linq;

namespace MainCore.Tasks.Sim
{
    public class InstantUpgrade : VillageBotTask
    {
        public InstantUpgrade(int villageId, int accountId) : base(villageId, accountId, "Complete upgrade queue")
        {
        }

        public override void Execute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
            NavigateHelper.Sleep(setting.ClickDelayMin, setting.ClickDelayMax);
            if (Cts.IsCancellationRequested) return;
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (!currentUrl.Contains("dorf"))
            {
                NavigateHelper.GoRandomDorf(_chromeBrowser, context, AccountId);
            }
            if (Cts.IsCancellationRequested) return;
            ClickHelper.ClickCompleteNow(_chromeBrowser);
            NavigateHelper.Sleep(setting.ClickDelayMin, setting.ClickDelayMax);

            if (Cts.IsCancellationRequested) return;
            ClickHelper.WaitDialogFinishNow(_chromeBrowser);
            NavigateHelper.Sleep(setting.ClickDelayMin, setting.ClickDelayMax);

            ClickHelper.ClickConfirmFinishNow(_chromeBrowser);
            NavigateHelper.Sleep(setting.ClickDelayMin, setting.ClickDelayMax);

            NavigateHelper.WaitPageLoaded(_chromeBrowser);
            NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);

            var tasks = _taskManager.GetList(AccountId);
            var upgradeTask = tasks.OfType<UpgradeBuilding>().FirstOrDefault(x => x.VillageId == VillageId);
            if (upgradeTask is not null)
            {
                upgradeTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(AccountId);
            }

            var updateTask = new UpdateVillage(VillageId, AccountId);
            updateTask.CopyFrom(this);
            updateTask.Execute();
        }
    }
}