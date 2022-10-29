using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using UI.ViewModels;

namespace UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.AccountTableViewModel, v => v.AccountTable.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.LoadingOverlayViewModel, v => v.LoadingOverlay.ViewModel).DisposeWith(d);
            });

            Opened += OnOpened; ;
        }

        private void OnOpened(object sender, EventArgs e)
        {
            ViewModel.InitServicesCommand.Execute().Subscribe();
        }
    }
}