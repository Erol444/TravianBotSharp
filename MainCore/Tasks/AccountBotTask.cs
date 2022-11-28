using MainCore.Helper.Implementations;

namespace MainCore.Tasks
{
    public abstract class AccountBotTask : BotTask
    {
        private readonly int _accountId;
        public int AccountId => _accountId;

        public AccountBotTask(int accountId, string name)
        {
            _accountId = accountId;
            Name = name;
        }

        public override void Refresh()
        {
            _chromeBrowser.GetChrome().Navigate().Refresh();
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.WaitPageLoaded(_chromeBrowser);
            NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
        }

        protected bool IsStop()
        {
            if (Cts.IsCancellationRequested) return true;
            if (StopFlag)
            {
                StopFlag = false;
                return true;
            }
            return false;
        }
    }
}