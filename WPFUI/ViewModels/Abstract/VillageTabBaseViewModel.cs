using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using WPFUI.Store;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class VillageTabBaseViewModel : ActivatableViewModelBase
    {
        protected readonly SelectedItemStore _selectedItemStore;

        protected abstract void Init(int villageId);

        public VillageTabBaseViewModel(SelectedItemStore selectedItemStore)
        {
            _selectedItemStore = selectedItemStore;

            _selectedItemStore.VillageChanged += OnVillageChanged;

            this.WhenAnyValue(vm => vm._selectedItemStore.Account)
                .WhereNotNull()
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.AccountId, out _accountId);

            this.WhenAnyValue(vm => vm._selectedItemStore.Village)
                .WhereNotNull()
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.VillageId, out _villageId);
        }

        protected override void OnActive()
        {
            if (!_selectedItemStore.IsVillageSelected) return;
            Init(VillageId);
        }

        private void OnVillageChanged(int villageId)
        {
            if (!IsActive) return;
            Init(villageId);
        }

        private readonly ObservableAsPropertyHelper<int> _accountId;

        public int AccountId => _accountId.Value;

        private readonly ObservableAsPropertyHelper<int> _villageId;

        public int VillageId => _villageId.Value;
    }
}