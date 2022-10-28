using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using UI.ViewModels;

namespace UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Greeting, v => v.Text.Text).DisposeWith(d);
            });
        }
    }
}