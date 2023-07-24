using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class AccountBotTask : BotTask
    {
        private readonly int _accountId;
        public int AccountId => _accountId;

        public AccountBotTask(int accountId, CancellationToken cancellationToken = default) : base(cancellationToken)
        {
            _accountId = accountId;
        }

        public override string GetName()
        {
            if (string.IsNullOrEmpty(_name))
            {
                _name = GetType().Name;
            }
            return _name;
        }
    }
}