using MainCore.Helper;
using System;
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
            using var context = _contextFactory.CreateDbContext();
            var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
            if (!adventures.Any()) return;
            var adventure = adventures.First();
            var x = adventure.X;
            var y = adventure.Y;
            if (Cts.IsCancellationRequested) return;
            ClickHelper.ClickStartAdventure(_chromeBrowser, x, y);
            if (DateTime.Now.Millisecond % 2 == 0)
            {
                NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
            }
            else
            {
                NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
            }
        }
    }
}