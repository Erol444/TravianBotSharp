using ReactiveUI;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class VillageTabBaseViewModel : TabBaseViewModel
    {
        public VillageTabBaseViewModel()
        {
            this.WhenAnyValue(vm => vm._selectorViewModel.Account)
                .Where(x => x is not null)
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.AccountId, out _accountId);

            this.WhenAnyValue(vm => vm._selectorViewModel.Village)
                .Where(x => x is not null)
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.VillageId, out _villageId);

            _selectorViewModel.VillageChanged += OnVillageChanged;
            Active += OnActive;
        }

        protected abstract void Init(int id);

        private void OnActive()
        {
            RxApp.TaskpoolScheduler.Schedule(() => Init(VillageId));
        }

        private void OnVillageChanged(int villageId)
        {
            RxApp.TaskpoolScheduler.Schedule(() => Init(villageId));
        }

        private readonly ObservableAsPropertyHelper<int> _accountId;

        public int AccountId
        {
            get => _accountId.Value;
        }

        private readonly ObservableAsPropertyHelper<int> _villageId;

        public int VillageId
        {
            get => _villageId.Value;
        }
    }
}