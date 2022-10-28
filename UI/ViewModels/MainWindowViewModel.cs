using ReactiveUI;
using System.Reactive.Disposables;

namespace UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
    {
        public MainWindowViewModel()
        {
            this.WhenActivated(disposables =>
            {
                HandleActivation();

                Disposable
                    .Create(() => HandleDeactivation())
                    .DisposeWith(disposables);
            });
        }

        private void HandleActivation()
        {
            _ = 2;
        }

        private void HandleDeactivation()
        {
            _ = 2;
        }

        public string Greeting => "Welcome to Avalonia!";

        public ViewModelActivator Activator { get; } = new();
    }
}