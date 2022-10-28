using ReactiveUI;
using System.Reactive.Disposables;

namespace UI.ViewModels
{
    public class ViewModelBase : ReactiveObject, IActivatableViewModel
    {
        public ViewModelBase()
        {
            this.WhenActivated(disposables =>
            {
                OnActived();

                Disposable
                    .Create(() => OnDeactived())
                    .DisposeWith(disposables);
            });
        }

        protected virtual void OnActived()
        {
        }

        protected virtual void OnDeactived()
        {
        }

        public ViewModelActivator Activator { get; } = new();
    }
}