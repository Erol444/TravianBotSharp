using ReactiveUI;
using Splat;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private bool _canClose = false;
        private bool _isClosing = false;

        public MainWindow()
        {
            ViewModel = Locator.Current.GetService<MainWindowViewModel>();
            ViewModel.Show = Show;
            Closing += OnClosing;

            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.AccountGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentAccount, v => v.AccountGrid.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentIndex, v => v.AccountGrid.SelectedIndex).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Tabs, v => v.Tabs.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TabIndex, v => v.Tabs.SelectedIndex).DisposeWith(d);

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
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