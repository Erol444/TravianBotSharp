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

            var taskUpdate = new UpdateInfo(AccountId);
            taskUpdate.CopyFrom(this);
            taskUpdate.Execute();

            var setting = context.AccountsSettings.Find(AccountId);
            if (setting.IsAutoAdventure)
            {
                var taskAutoSend = new StartAdventure(AccountId);
                taskAutoSend.CopyFrom(this);
                taskAutoSend.Execute();
                taskUpdate.Execute();
            }
        }
    }
}