using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels
{
    public class ActivatableViewModelBase : TabBaseViewModel, IActivatableViewModel
    {
        public ActivatableViewModelBase()
        {
            this.WhenActivated(disposables =>
            {
                OnActived();

                Disposable
                    .Create(() => OnDeactived())
                    .DisposeWith(disposables);
            });

            _selectorViewModel.AccountChanged += AccountChange;
            _selectorViewModel.VillageChanged += VillageChange;

            this.WhenAnyValue(vm => vm._selectorViewModel.Account)
                .Where(x => x is not null)
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.AccountId, out _accountId);
            this.WhenAnyValue(vm => vm._selectorViewModel.Village)
                .Where(x => x is not null)
                .Select(x => x.Id)
                .ToProperty(this, vm => vm.VillageId, out _villageId);
        }

        protected event Action OnActive;

        protected event Action OnDeactive;

        protected event Action<int> OnAccountChange;

        private void AccountChange(int accountId) => OnAccountChange?.Invoke(accountId);

        protected event Action<int> OnVillageChange;

        private void VillageChange(int villageId) => OnVillageChange?.Invoke(villageId);

        protected bool IsActive { get; private set; }

        private void OnActived()
        {
            IsActive = true;
            RxApp.MainThreadScheduler.Schedule(() => OnActive?.Invoke());
        }

        private void OnDeactived()
        {
            IsActive = false;
            RxApp.MainThreadScheduler.Schedule(() => OnDeactive?.Invoke());
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

        public ViewModelActivator Activator { get; } = new();
    }
}