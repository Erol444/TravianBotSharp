using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class ActivatableViewModelBase : ReactiveObject, IActivatableViewModel
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
        }

        protected event Action Active;

        protected event Action Deactive;

        protected bool IsActive { get; private set; }

        private void OnActived()
        {
            IsActive = true;
            RxApp.TaskpoolScheduler.Schedule(() => Active?.Invoke());
        }

        private void OnDeactived()
        {
            IsActive = false;
            RxApp.TaskpoolScheduler.Schedule(() => Deactive?.Invoke());
        }

        public ViewModelActivator Activator { get; } = new();
    }
}