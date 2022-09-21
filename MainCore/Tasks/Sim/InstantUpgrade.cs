using MainCore.Helper;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Sim
{
    public class InstantUpgrade : BotTask
    {
        public InstantUpgrade(int villageId, int accountId) : base(accountId)
        {
            _villageId = villageId;
        }

        private readonly int _villageId;
        public int VillageId => _villageId;
        private string _name;
        public override string Name => _name;
        private readonly Random rand = new();

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Complete upgrading in {VillageId}";
            }
            else
            {
                _name = $"Complete upgrading in {village.Name}";
            }
        }

        public override void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IEventManager eventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            base.SetService(contextFactory, chromeBrowser, taskManager, eventManager, logManager, planManager, restClientManager);
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Complete upgrading in {VillageId}";
            }
            else
            {
                _name = $"Complete upgrading in {village.Name}";
            }
        }

        public override void Execute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
            var delay = rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);
            if (Cts.IsCancellationRequested) return;
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (!currentUrl.Contains("dorf"))
            {
                NavigateHelper.GoRandomDorf(_chromeBrowser, context, AccountId);
            }
            if (Cts.IsCancellationRequested) return;
            ClickHelper.ClickCompleteNow(_chromeBrowser);
            delay = rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);
            if (Cts.IsCancellationRequested) return;
            ClickHelper.WaitDialogFinishNow(_chromeBrowser);
            delay = rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);
            ClickHelper.ClickConfirmFinishNow(_chromeBrowser);
            delay = rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);

            NavigateHelper.WaitPageLoaded(_chromeBrowser);
            NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);

            var tasks = _taskManager.GetList(AccountId);
            var upgradeTask = tasks.Where(x => x.GetType() == typeof(UpgradeBuilding)).OfType<UpgradeBuilding>().FirstOrDefault(x => x.VillageId == VillageId);
            if (upgradeTask is not null)
            {
                upgradeTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(AccountId);
            }
        }
    }
}