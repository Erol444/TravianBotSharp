using ReactiveUI;
using System.Reactive.Disposables;
using System.Threading;

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

        protected virtual void OnActived(CompositeDisposable disposable)
        {
            _cancellationTokenSource = new();
            IsActive = true;
        }

        protected virtual void OnDeactived()
        {
            IsActive = false;
            _cancellationTokenSource.Cancel();
        }

        public ViewModelActivator Activator { get; } = new();
        protected CancellationTokenSource _cancellationTokenSource;
    }
}