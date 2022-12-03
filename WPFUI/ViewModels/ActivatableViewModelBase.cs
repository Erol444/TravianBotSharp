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

            this.WhenAnyValue(vm => vm._selectorViewModel.Account).Subscribe((x) =>
            {
                if (x is null) return;
                RxApp.MainThreadScheduler.Schedule(() => OnAccountChange?.Invoke(x.Id));
            });

            this.WhenAnyValue(vm => vm._selectorViewModel.Village).Subscribe((x) =>
            {
                if (x is null) return;
                RxApp.MainThreadScheduler.Schedule(() => OnVillageChange?.Invoke(x.Id));
            });
        }

        protected event Action OnActive;

        protected event Action OnDeactive;

        protected event Action<int> OnAccountChange;

        protected event Action<int> OnVillageChange;

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

        public ViewModelActivator Activator { get; } = new();
    }
}