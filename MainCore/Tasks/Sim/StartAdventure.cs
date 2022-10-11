using MainCore.Helper;
using MainCore.Models.Database;
using System.Linq;

namespace MainCore.Tasks.Sim
{
    public class StartAdventure : AccountBotTask
    {
        public StartAdventure(int accountId) : base(accountId, "Start adventure")
        {
        }

        public override void Execute()
        {
            var adventure = GetAdventures();
            if (StopFlag) return;
            if (Cts.IsCancellationRequested) return;
            if (adventure is null) return;
            StartAdventures(adventure);
            if (StopFlag) return;
            if (Cts.IsCancellationRequested) return;
        }

        private Adventure GetAdventures()
        {
            using var context = _contextFactory.CreateDbContext();
            var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
            return adventures.FirstOrDefault();
        }

        private void StartAdventures(Adventure adventure)
        {
            var x = adventure.X;
            var y = adventure.Y;
            ClickHelper.ClickStartAdventure(_chromeBrowser, x, y);
        }
    }
}