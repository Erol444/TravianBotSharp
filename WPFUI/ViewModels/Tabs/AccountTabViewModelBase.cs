using ReactiveUI;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace WPFUI.ViewModels.Tabs
{
    public abstract class AccountTabViewModelBase : ActivatableViewModelBase
    {
        public AccountTabViewModelBase()
        {
            this.WhenAnyValue(vm => vm._selectorViewModel.Account)
                .Where(x => x is not null)
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.AccountId, out _accountId);

            _selectorViewModel.AccountChanged += OnAccountChanged;
            Active += OnActive;
        }

        private void OnActive()
        {
            RxApp.TaskpoolScheduler.Schedule(() => Init(AccountId));
        }

        private void OnAccountChanged(int accountId)
        {
            RxApp.TaskpoolScheduler.Schedule(() => Reload(accountId));
        }

        private readonly ObservableAsPropertyHelper<int> _accountId;

        public int AccountId
        {
            get => _accountId.Value;
        }
    }
}