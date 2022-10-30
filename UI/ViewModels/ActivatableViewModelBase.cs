using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace UI.ViewModels
{
    /// <summary>
    /// Base for Transient ViewModel
    /// </summary>
    public class ActivatableViewModelBase : ViewModelBase, IActivatableViewModel
    {
        public ActivatableViewModelBase()
        {
            this.WhenActivated(disposables =>
            {
                OnActived(disposables);

                Disposable
                    .Create(() => OnDeactived())
                    .DisposeWith(disposables);
            });
        }

        protected bool IsActive { get; private set; }

        protected virtual void OnActived(IDisposable disposable)
        {
            IsActive = true;
        }

        protected virtual void OnDeactived()
        {
            IsActive = false;
        }

        public ViewModelActivator Activator { get; } = new();
    }
}