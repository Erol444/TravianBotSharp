using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using WPFUI.Store;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class AccountTabBaseViewModel : TabBaseViewModel
    {
        protected readonly SelectedItemStore _selectedItemStore;

        protected abstract void Init(int accountId);

        public AccountTabBaseViewModel(SelectedItemStore selectedItemStore)
        {
            _selectedItemStore = selectedItemStore;

            _selectedItemStore.AccountChanged += OnAccountChanged;

            this.WhenAnyValue(vm => vm._selectedItemStore.Account)
                .WhereNotNull()
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.AccountId, out _accountId);
        }

        protected override void OnActive()
        {
            if (!_selectedItemStore.IsAccountSelected) return;
            Init(AccountId);
        }

        private void OnAccountChanged(int accountId)
        {
            if (!IsActive) return;
            Init(accountId);
        }

        private readonly ObservableAsPropertyHelper<int> _accountId;
        public int AccountId => _accountId.Value;
    }
}