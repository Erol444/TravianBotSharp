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

                this.Bind(ViewModel, vm => vm.TabSelector, v => v.ButtonPanel.ViewModel.TabSelector).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TabSelector, v => v.AddAccountPage.ViewModel.TabSelector).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TabSelector, v => v.AddAccountsPage.ViewModel.TabSelector).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TabSelector, v => v.EditAccountPage.ViewModel.TabSelector).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.GeneralPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.SettingsPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.HeroPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.VillagesPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.FarmingPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.DebugPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.EditAccountPage.ViewModel.CurrentAccount).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Selector.SelectedNoAccount, v => v.NoAccountTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Selector.SelectedNormal, v => v.DebugTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Selector.SelectedAddAccount, v => v.AddAccountTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Selector.SelectedAddAccounts, v => v.AddAccountsTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Selector.SelectedEditAccount, v => v.EditAccountTab.IsSelected).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowNoAccountTab, v => v.NoAccountTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowNormalTab, v => v.GeneralTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowNormalTab, v => v.SettingsTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowNormalTab, v => v.HeroTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowNormalTab, v => v.VillagesTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowNormalTab, v => v.FarmingTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowNormalTab, v => v.DebugTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowAddAccountTab, v => v.AddAccountTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowAddAccountsTab, v => v.AddAccountsTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ShowDecider.ShowEditAccountTab, v => v.EditAccountTab.Visibility).DisposeWith(d);

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