using MainCore.Models.Database;
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
            this.WhenAnyValue(vm => vm.Account).Subscribe(x =>
            {
                if (x is null) return;

                RxApp.TaskpoolScheduler.Schedule(() => OnAccountChanged(x.Id));
            });
            this.WhenAnyValue(vm => vm.Village).Subscribe(x =>
            {
                if (x is null) return;

                RxApp.TaskpoolScheduler.Schedule(() => OnVillageChanged(x.Id));
            });

            this.WhenAnyValue(vm => vm.Account).Select(x => x is not null).ToProperty(this, vm => vm.IsAccountSelected, out _isAccountSelected);
            this.WhenAnyValue(vm => vm.IsAccountSelected).Select(x => !x).ToProperty(this, vm => vm.IsAccountNotSelected, out _isAccountNotSelected);

            this.WhenAnyValue(vm => vm.Village).Select(x => x is not null).ToProperty(this, vm => vm.IsVillageSelected, out _isVillageSelected);
            this.WhenAnyValue(vm => vm.IsVillageSelected).Select(x => !x).ToProperty(this, vm => vm.IsVillageNotSelected, out _isVillageNotSelected);
        }

        public event Action<int> AccountChanged;

        private void OnAccountChanged(int account) => VillageChanged?.Invoke(account);

        public event Action<int> VillageChanged;

        private void OnVillageChanged(int village) => VillageChanged?.Invoke(village);

        private Account _account;

        public Account Account
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
    }
}