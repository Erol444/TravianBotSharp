namespace MainCore.Tasks
{
    public abstract class AccountBotTask : BotTask
    {
        private int _accountId;
        public int AccountId => _accountId;

        public void SetAccountId(int accountId)
        {
            _accountId = accountId;
        }
    }
}