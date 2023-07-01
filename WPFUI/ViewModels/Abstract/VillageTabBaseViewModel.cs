using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using WPFUI.Store;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class VillageTabBaseViewModel : ActivatableViewModelBase
    {
        protected readonly SelectedItemStore _selectedItemStore;

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

        protected abstract void Init(int id);

        protected override void OnActive()
        {
            Init(VillageId);
        }

        private void OnVillageChanged(int villageId)
        {
            Init(villageId);
        }

        private readonly ObservableAsPropertyHelper<int> _accountId;

        public int AccountId => _accountId.Value;

        private readonly ObservableAsPropertyHelper<int> _villageId;

        public int VillageId => _villageId.Value;
    }
}