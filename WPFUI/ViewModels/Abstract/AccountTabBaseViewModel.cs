using ReactiveUI;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class AccountTabBaseViewModel : TabBaseViewModel
    {
        public AccountTabBaseViewModel()
        {
            this.WhenAnyValue(vm => vm._selectorViewModel.Account)
                .Where(x => x is not null)
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.AccountId, out _accountId);

            _selectorViewModel.AccountChanged += OnAccountChanged;
            Active += OnActive;
        }

        protected abstract void Init(int id);

        private void OnActive()
        {
            RxApp.TaskpoolScheduler.Schedule(() => Init(AccountId));
        }

        private void OnAccountChanged(int accountId)
        {
            if (!IsActive) return;
            RxApp.TaskpoolScheduler.Schedule(() => Init(accountId));
        }

        private readonly ObservableAsPropertyHelper<int> _accountId;

        public int AccountId
        {
            get => _accountId.Value;
        }
    }
}