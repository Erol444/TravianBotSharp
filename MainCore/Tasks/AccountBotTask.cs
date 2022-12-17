namespace MainCore.Tasks
{
    public abstract class AccountBotTask : BotTask
    {
        private readonly int _accountId;
        public int AccountId => _accountId;

        public AccountBotTask(int accountId)
        {
            _accountId = accountId;
            _chromeBrowser = _chromeManager.Get(accountId);
        }

        public override string GetName()
        {
            if (string.IsNullOrEmpty(_name))
            {
                _name = GetType().ToString();
            }
            return _name;
        }
    }
}