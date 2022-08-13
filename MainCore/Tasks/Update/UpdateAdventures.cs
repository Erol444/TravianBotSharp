using MainCore.Helper;

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
            using var context = ContextFactory.CreateDbContext();
            UpdateHelper.UpdateAdventures(context, ChromeBrowser, AccountId);
        }
    }
}