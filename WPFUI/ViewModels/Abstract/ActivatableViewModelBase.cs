using ReactiveUI;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class ActivatableViewModelBase : ViewModelBase, IActivatableViewModel
    {
        public ActivatableViewModelBase()
        {
            this.WhenActivated(disposables =>
            {
                IsActive = true;
                RxApp.TaskpoolScheduler.Schedule(OnActive);
                Disposable
                    .Create(() =>
                    {
                        IsActive = false;
                        RxApp.TaskpoolScheduler.Schedule(OnDeactive);
                    })
                    .DisposeWith(disposables);
            });
        }

        protected bool IsActive { get; private set; }

        protected virtual void OnActive()
        {
        }

        protected virtual void OnDeactive()
        {
        }

        public ViewModelActivator Activator { get; } = new();
    }
}