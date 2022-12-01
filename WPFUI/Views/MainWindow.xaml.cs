using ReactiveUI;
using System;
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
        public MainWindow()
        {
            ViewModel = new();

            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Events().Closing.InvokeCommand(this, x => x.ViewModel.ClosingCommand).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.AccountGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentAccount, v => v.AccountGrid.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentIndex, v => v.AccountGrid.SelectedIndex).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsAccountSelected, v => v.ButtonPanel.ViewModel.IsAccountSelected).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.ButtonPanel.ViewModel.CurrentAccount).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Tabs, v => v.Tabs.ItemsSource).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.GeneralPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.SettingsPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.HeroPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.VillagesPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.FarmingPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.DebugPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.EditAccountPage.ViewModel.CurrentAccount).DisposeWith(d);

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}