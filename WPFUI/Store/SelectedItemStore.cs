using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;

namespace WPFUI.Store
{
    public class SelectedItemStore : ReactiveObject
    {
        public SelectedItemStore()
        {
            var accountObservable = this.WhenAnyValue(vm => vm.Account);
            accountObservable
                .Select(x => x is not null)
                .ToProperty(this, vm => vm.IsAccountSelected, out _isAccountSelected);
            accountObservable
                .Select(x => x is null)
                .ToProperty(this, vm => vm.IsAccountNotSelected, out _isAccountNotSelected);
            accountObservable
                .WhereNotNull()
                .Subscribe(x => RxApp.TaskpoolScheduler.Schedule(() => OnAccountChanged(x.Id)));

            var villageObservable = this.WhenAnyValue(vm => vm.Village);
            villageObservable
                .Select(x => x is not null)
                .ToProperty(this, vm => vm.IsVillageSelected, out _isVillageSelected);
            villageObservable
                .Select(x => x is null)
                .ToProperty(this, vm => vm.IsVillageNotSelected, out _isVillageNotSelected);
            villageObservable
                .WhereNotNull()
                .Subscribe(x => RxApp.TaskpoolScheduler.Schedule(() => OnVillageChanged(x.Id)));
        }

        public event Action<int> AccountChanged;

        private void OnAccountChanged(int account) => AccountChanged?.Invoke(account);

        public event Action<int> VillageChanged;

        private void OnVillageChanged(int village) => VillageChanged?.Invoke(village);

        private ListBoxItem _account;

        public ListBoxItem Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountSelected;
        private readonly ObservableAsPropertyHelper<bool> _isAccountNotSelected;

        public bool IsAccountSelected => _isAccountSelected.Value;
        public bool IsAccountNotSelected => _isAccountNotSelected.Value;

        private ListBoxItem _village;

        public ListBoxItem Village
        {
            get => _village;
            set => this.RaiseAndSetIfChanged(ref _village, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isVillageSelected;
        private readonly ObservableAsPropertyHelper<bool> _isVillageNotSelected;

        public bool IsVillageSelected => _isVillageSelected.Value;
        public bool IsVillageNotSelected => _isVillageNotSelected.Value;
    }
}