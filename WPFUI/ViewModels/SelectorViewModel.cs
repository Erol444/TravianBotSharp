using System;
using System.Linq;

namespace WPFUI.ViewModels
{
    public class SelectorViewModel : ReactiveObject
    {
        public SelectorViewModel()
        {
            this.WhenAnyValue(vm => vm.Account)
                .Where(x => x is not null)
                .Subscribe(x => RxApp.TaskpoolScheduler.Schedule(() => OnAccountChanged(x.Id)));
            this.WhenAnyValue(vm => vm.Village)
               .Where(x => x is not null)
               .Subscribe(x => RxApp.TaskpoolScheduler.Schedule(() => OnVillageChanged(x.Id)));
            this.WhenAnyValue(vm => vm.Farm)
               .Where(x => x is not null)
               .Subscribe(x => RxApp.TaskpoolScheduler.Schedule(() => OnFarmChanged(x.Id)));

            this.WhenAnyValue(vm => vm.Account)
                .Select(x => x is not null)
                .ToProperty(this, vm => vm.IsAccountSelected, out _isAccountSelected);
            this.WhenAnyValue(vm => vm.Account)
                .Select(x => x is null)
                .ToProperty(this, vm => vm.IsAccountNotSelected, out _isAccountNotSelected);

            this.WhenAnyValue(vm => vm.Village)
                .Select(x => x is not null)
                .ToProperty(this, vm => vm.IsVillageSelected, out _isVillageSelected);
            this.WhenAnyValue(vm => vm.Village)
                .Select(x => x is null)
                .ToProperty(this, vm => vm.IsVillageNotSelected, out _isVillageNotSelected);

            this.WhenAnyValue(vm => vm.Farm)
                .Select(x => x is not null)
                .ToProperty(this, vm => vm.IsFarmSelected, out _isFarmSelected);
            this.WhenAnyValue(vm => vm.Farm)
                .Select(x => x is null)
                .ToProperty(this, vm => vm.IsFarmNotSelected, out _isFarmNotSelected);
        }

        public event Action<int> AccountChanged;

        private void OnAccountChanged(int account) => AccountChanged?.Invoke(account);

        public event Action<int> VillageChanged;

        private void OnVillageChanged(int village) => VillageChanged?.Invoke(village);

        public event Action<int> FarmChanged;

        private void OnFarmChanged(int farm) => FarmChanged?.Invoke(farm);

        private ListBoxItem _account;

        public ListBoxItem Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
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

        private VillageModel _village;

        public VillageModel Village
        {
            get => _village;
            set => this.RaiseAndSetIfChanged(ref _village, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isVillageSelected;

        public bool IsVillageSelected
        {
            get => _isVillageSelected.Value;
        }

        private readonly ObservableAsPropertyHelper<bool> _isVillageNotSelected;

        public bool IsVillageNotSelected
        {
            get => _isVillageNotSelected.Value;
        }

        private ListBoxItem _farm;

        public ListBoxItem Farm
        {
            get => _farm;
            set => this.RaiseAndSetIfChanged(ref _farm, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isFarmSelected;

        public bool IsFarmSelected
        {
            get => _isFarmSelected.Value;
        }

        private readonly ObservableAsPropertyHelper<bool> _isFarmNotSelected;

        public bool IsFarmNotSelected
        {
            get => _isFarmNotSelected.Value;
        }
    }
}