using ReactiveUI;
using System;
using System.Windows;

namespace WPFUI
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

            #region Commands

            this.BindCommand(ViewModel,
                vm => vm.CheckVersionCommand,
                v => v.CheckVersionButton
            );
            this.BindCommand(ViewModel,
                vm => vm.AddAccountCommand,
                v => v.AddAccountButton
            );
            this.BindCommand(ViewModel,
               vm => vm.AddAccountsCommand,
               v => v.AddAccountsButton
           );
            this.BindCommand(ViewModel,
               vm => vm.LoginCommand,
               v => v.LoginButton
           );
            this.BindCommand(ViewModel,
               vm => vm.LogoutCommand,
               v => v.LogoutButton
           );
            this.BindCommand(ViewModel,
               vm => vm.EditAccountCommand,
               v => v.EditButton
           );
            this.BindCommand(ViewModel,
               vm => vm.DeleteAccountCommand,
               v => v.DeleteButton
           );
            this.BindCommand(ViewModel,
               vm => vm.LoginAllCommand,
               v => v.LoginAllButton
           );
            this.BindCommand(ViewModel,
               vm => vm.LogoutAllCommand,
               v => v.LogoutAllButton
           );

            #endregion Commands

            #region Events

            this.Events().Closing.InvokeCommand(this, x => x.ViewModel.ClosingCommand);

            #endregion Events

            #region Data

            this.OneWayBind(ViewModel,
                vm => vm.Accounts,
                v => v.AccountGrid.ItemsSource);

            this.Bind(ViewModel,
                vm => vm.CurrentAccount,
                v => v.AccountGrid.SelectedItem);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountNotSelected,
                v => v.NoAccountTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.GeneralTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.HeroTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.OverviewTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.VillagesTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.DebugTab.Visibility);

            #endregion Data
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}