using MainCore.Helper;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;

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

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Complete upgrading in Unknow {village.Id}";
            }
            else
            {
                _name = $"Complete upgrading in {village.Name}";
            }
        }

        public override void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IEventManager eventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            base.SetService(contextFactory, chromeBrowser, taskManager, eventManager, logManager, planManager, restClientManager);
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Complete upgrading in Unknow {village.Id}";
            }
            else
            {
                _name = $"Complete upgrading in {village.Name}";
            }
        }

        public override void Execute()
        {
            using var context = ContextFactory.CreateDbContext();
            NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
            if (Cts.IsCancellationRequested) return;
            NavigateHelper.GoRandomDorf(ChromeBrowser);
            if (Cts.IsCancellationRequested) return;
            ClickHelper.ClickCompleteNow(ChromeBrowser);
            if (Cts.IsCancellationRequested) return;
            ClickHelper.WaitDialogFinishNow(ChromeBrowser);
        }
    }
}