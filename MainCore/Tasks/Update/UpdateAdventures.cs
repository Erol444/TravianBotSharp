using MainCore.Enums;
using MainCore.Helper;
using MainCore.Tasks.Sim;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateAdventures : BotTask
    {
        public UpdateAdventures(int accountId) : base(accountId)
        {
        }

        public override string Name => "Update hero's adventures";

        public override void Execute()
        {
            NavigateHelper.ToAdventure(ChromeBrowser);
            if (Cts.IsCancellationRequested) return;
            using var context = ContextFactory.CreateDbContext();
            UpdateHelper.UpdateAdventures(context, ChromeBrowser, AccountId);

            var taskUpdate = new UpdateInfo(AccountId);
            this.CopyTo(taskUpdate);
            taskUpdate.Execute();

            var hero = context.Heroes.Find(AccountId);
            var setting = context.AccountsSettings.Find(AccountId);
            if (hero.Status == HeroStatusEnums.Home && setting.IsAutoAdventure)
            {
                var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
                if (!adventures.Any()) return;
                TaskManager.Add(AccountId, new StartAdventure(AccountId));
            }
        }
    }
}