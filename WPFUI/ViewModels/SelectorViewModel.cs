using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;

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

            this.WhenAnyValue(vm => vm.Building)
               .Where(x => x is not null)
               .Subscribe(x => RxApp.TaskpoolScheduler.Schedule(() => OnBuildingChanged(x.Id)));

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

            this.WhenAnyValue(vm => vm.Building)
                .Select(x => x is not null)
                .ToProperty(this, vm => vm.IsBuildingSelected, out _isBuildingSelected);
            this.WhenAnyValue(vm => vm.Building)
                .Select(x => x is null)
                .ToProperty(this, vm => vm.IsBuildingNotSelected, out _isBuildingNotSelected);

            this.WhenAnyValue(vm => vm.Queue)
                .Select(x => x is not null)
                .ToProperty(this, vm => vm.IsQueueSelected, out _isQueueSelected);
            this.WhenAnyValue(vm => vm.Farm)
                .Select(x => x is null)
                .ToProperty(this, vm => vm.IsQueueNotSelected, out _isQueueNotSelected);
        }

        public event Action<int> AccountChanged;

        private void OnAccountChanged(int account) => AccountChanged?.Invoke(account);

        public event Action<int> VillageChanged;

        private void OnVillageChanged(int village) => VillageChanged?.Invoke(village);

        public event Action<int> FarmChanged;

        private void OnFarmChanged(int farm) => FarmChanged?.Invoke(farm);

        public event Action<int> BuildingChanged;

        private void OnBuildingChanged(int building) => BuildingChanged?.Invoke(building);

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

        private ListBoxItem _building;

        public ListBoxItem Building
        {
            get => _building;
            set => this.RaiseAndSetIfChanged(ref _building, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isBuildingSelected;

        public bool IsBuildingSelected
        {
            get => _isBuildingSelected.Value;
        }

        private readonly ObservableAsPropertyHelper<bool> _isBuildingNotSelected;

        public bool IsBuildingNotSelected
        {
            get => _isBuildingNotSelected.Value;
        }

        private ListBoxItem _queue;

        public ListBoxItem Queue
        {
            get => _queue;
            set => this.RaiseAndSetIfChanged(ref _queue, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isQueueSelected;

        public bool IsQueueSelected
        {
            get => _isQueueSelected.Value;
        }

        private readonly ObservableAsPropertyHelper<bool> _isQueueNotSelected;

        public bool IsQueueNotSelected
        {
            get => _isQueueNotSelected.Value;
        }
    }
}