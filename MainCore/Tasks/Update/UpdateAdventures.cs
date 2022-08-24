using MainCore.Enums;
using MainCore.Helper;
using MainCore.Tasks.Sim;

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

            var hero = context.Heroes.Find(AccountId);
            var setting = context.AccountsSettings.Find(AccountId);
            if (hero.Status == HeroStatusEnums.Home && setting.IsAutoAdventure)
            {
                TaskManager.Add(AccountId, new StartAdventure(AccountId));
            }
        }
    }
}