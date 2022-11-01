using Avalonia.ReactiveUI;
using Splat;
using System;
using UI.ViewModels;

namespace UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            ViewModel = Locator.Current.GetService<MainWindowViewModel>();
            InitializeComponent();
            Opened += OnOpened; ;
        }

        private void OnOpened(object sender, EventArgs e)
        {
            ViewModel.InitServicesCommand.Execute().Subscribe();
        }
    }
}