using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace UI.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        public AccountViewModel(IEventManager eventManager, ITaskManager taskManager) : base()
        {
            this.WhenAnyValue(vm => vm.Account).Subscribe(x =>
            {
                if (x is null) return;
                Status = taskManager.GetAccountStatus(x.Id);

                OnAccountChanged(x.Id);
                OnStatusChanged(Status);
            });

            this.WhenAnyValue(vm => vm.Account).Select(x => x is not null).ToProperty(this, vm => vm.IsAccountSelected, out _isAccountSelected);
            this.WhenAnyValue(vm => vm.IsAccountSelected).Select(x => !x).ToProperty(this, vm => vm.IsAccountNotSelected, out _isAccountNotSelected);

            eventManager.AccountStatusUpdate += OnAccountStatusUpdate;
        }

        private void OnAccountStatusUpdate(int accountId, AccountStatus status)
        {
            if (Account is null) return;
            if (Account.Id != accountId) return;
            Status = status;
            OnStatusChanged(status);
        }

        public event Action<int> AccountChanged;

        public event Action<AccountStatus> StatusChanged;

        private void OnAccountChanged(int accountId) => AccountChanged?.Invoke(accountId);

        private void OnStatusChanged(AccountStatus status) => StatusChanged?.Invoke(status);

        private Account _account;

        public Account Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
        }

        private AccountStatus _status;

        public AccountStatus Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountSelected;

        public bool IsAccountSelected
        {
            get => _isAccountSelected.Value;
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountNotSelected;

        public bool IsAccountNotSelected
        {
            get => _isAccountNotSelected.Value;
        }
    }
}