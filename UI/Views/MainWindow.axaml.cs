using Avalonia.ReactiveUI;
using Splat;
using System;
using System.ComponentModel;
using UI.ViewModels;

namespace UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private bool _canClose = false;
        private bool _isClosing = false;

        public MainWindow()
        {
            ViewModel = Locator.Current.GetService<MainWindowViewModel>();
            InitializeComponent();
            Opened += OnOpened;
            Closing += OnClosing;
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            if (_canClose) return;
            e.Cancel = true;
            if (_isClosing) return;
            _isClosing = true;

            await ViewModel.ClosingTask(e);

            _canClose = true;
            Close();
        }

        private void OnOpened(object sender, EventArgs e)
        {
            ViewModel.InitServicesCommand.Execute().Subscribe();
        }
    }
}