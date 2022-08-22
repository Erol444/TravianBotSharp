using MainCore.Helper;

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
        public override string Name => $"Instant upgrade buinding village {VillageId}";

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