using ReactiveUI;
using Splat;
using System;
using System.ComponentModel;
using System.Windows;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    public class MainWindowBase : ReactiveWindow<MainWindowViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MainWindowBase
    {
        private bool _canClose = false;
        private bool _isClosing = false;

        public MainWindow()
        {
            ViewModel = Locator.Current.GetService<MainWindowViewModel>();
            ViewModel.Show = Show;
            Closing += OnClosing;

            InitializeComponent();
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}