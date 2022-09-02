using ReactiveUI;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for ButtonPanelUc.xaml
    /// </summary>
    public partial class ButtonPanelUc : ReactiveUserControl<ButtonPanelViewModel>
    {
        public ButtonPanelUc()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
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
                  vm => vm.SettingsAccountCommand,
                  v => v.SettingsButton
              );
                this.BindCommand(ViewModel,
                   vm => vm.LoginAllCommand,
                   v => v.LoginAllButton
               );
                this.BindCommand(ViewModel,
                   vm => vm.LogoutAllCommand,
                   v => v.LogoutAllButton
               );
            });
        }
    }
}