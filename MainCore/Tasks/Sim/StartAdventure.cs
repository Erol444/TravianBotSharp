using MainCore.Helper;
using MainCore.Tasks.Update;
using System;
using System.Linq;

namespace MainCore.Tasks.Sim
{
    public class StartAdventure : UpdateAdventures
    {
        public StartAdventure(int accountId) : base(accountId)
        {
        }

        public override string Name => "Start adventure";

        public override void Execute()
        {
            base.Execute();
            using var context = ContextFactory.CreateDbContext();
            var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
            if (!adventures.Any()) return;
            var adventure = adventures.First();
            var x = adventure.X;
            var y = adventure.Y;
            if (Cts.IsCancellationRequested) return;
            ClickHelper.ClickStartAdventure(ChromeBrowser, x, y);
            if (DateTime.Now.Millisecond % 2 == 0)
            {
                NavigateHelper.ToDorf1(ChromeBrowser);
            }
            else
            {
                NavigateHelper.ToDorf2(ChromeBrowser);
            }

            var taskUpdate = new UpdateInfo(AccountId);
            this.CopyTo(taskUpdate);
            taskUpdate.Execute();
        }
    }
}