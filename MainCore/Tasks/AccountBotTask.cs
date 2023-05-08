using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Splat;
using System;
using System.Threading;

namespace MainCore.Tasks
{
    public abstract class AccountBotTask : BotTask
    {
        private readonly int _accountId;
        public int AccountId => _accountId;
        protected readonly IChromeBrowser _chromeBrowser;
        protected readonly IGeneralHelper _generalHelper;

        public AccountBotTask(int accountId, CancellationToken cancellationToken = default) : base(cancellationToken)
        {
            _accountId = accountId;
            _chromeBrowser = _chromeManager.Get(accountId);
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
        }

        public override string GetName()
        {
            if (string.IsNullOrEmpty(_name))
            {
                _name = GetType().Name;
            }
            return _name;
        }

        public void RefreshChrome()
        {
            _chromeBrowser.Navigate();
            if (DateTime.Now.Millisecond % 10 > 5)
            {
                _generalHelper.ToDorf1(AccountId);
            }
            else
            {
                _generalHelper.ToDorf2(AccountId);
            }
        }
    }
}