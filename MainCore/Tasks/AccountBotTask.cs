namespace MainCore.Tasks
{
    public abstract class AccountBotTask : BotTask
    {
        private readonly int _accountId;
        public int AccountId => _accountId;

        public AccountBotTask(int accountId, string name)
        {
            _accountId = accountId;
            _chromeBrowser = _chromeManager.Get(accountId);
            Name = name;
        }
    }
}