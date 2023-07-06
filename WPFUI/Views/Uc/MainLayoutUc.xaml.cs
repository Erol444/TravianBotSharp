using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    public partial class MainLayoutUcBase : ReactiveUserControl<MainLayoutViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MainLayoutUc.xaml
    /// </summary>
    public partial class MainLayoutUc : MainLayoutUcBase
    {
        public MainLayoutUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.CheckVersionCommand, v => v.CheckVersionButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddAccountCommand, v => v.AddAccountButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddAccountsCommand, v => v.AddAccountsButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.LoginButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.LogoutButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAccountCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.PauseCommand, v => v.PauseButton).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TextPause, v => v.PauseButton.Content).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RestartCommand, v => v.RestartButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.AccountGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentAccount, v => v.AccountGrid.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.NoAccountViewModel, v => v.NoAccount.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AddAccountViewModel, v => v.AddAccount.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AddAccountsViewModel, v => v.AddAccounts.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.SettingsViewModel, v => v.Settings.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.HeroViewModel, v => v.Hero.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VillagesViewModel, v => v.Villages.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.FarmingViewModel, v => v.Farming.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.EditAccountViewModel, v => v.EditAccount.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DebugViewModel, v => v.Debug.ViewModel).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsNoAccountTabVisible, v => v.NoAccountTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsAddAccountTabVisible, v => v.AddAccountTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsAddAccountsTabVisible, v => v.AddAccountsTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsNormalTabVisible, v => v.SettingsTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsNormalTabVisible, v => v.HeroTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsNormalTabVisible, v => v.VillagesTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsNormalTabVisible, v => v.FarmingTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsNormalTabVisible, v => v.EditAccountTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountTabStore.IsNormalTabVisible, v => v.DebugTab.Visibility).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.AccountTabStore.IsNoAccountTabSelected, v => v.NoAccountTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AccountTabStore.IsAddAccountTabSelected, v => v.AddAccountTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AccountTabStore.IsAddAccountsTabSelected, v => v.AddAccountsTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AccountTabStore.IsNormalTabSelected, v => v.SettingsTab.IsSelected).DisposeWith(d);
            });
        }
    }
}