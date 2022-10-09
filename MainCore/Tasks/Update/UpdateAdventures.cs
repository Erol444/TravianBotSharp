﻿using MainCore.Helper;
using MainCore.Tasks.Sim;

namespace MainCore.Tasks.Update
{
    public class UpdateAdventures : AccountBotTask
    {
        public UpdateAdventures(int accountId) : base(accountId, "Update hero's adventures")
        {
        }

        public override void Execute()
        {
            using var context = _contextFactory.CreateDbContext();

            NavigateHelper.ToAdventure(_chromeBrowser);
            if (Cts.IsCancellationRequested) return;
            UpdateHelper.UpdateAdventures(context, _chromeBrowser, AccountId);

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