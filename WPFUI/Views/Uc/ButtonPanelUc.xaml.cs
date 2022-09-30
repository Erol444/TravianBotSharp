using ReactiveUI;
using System.Reactive.Disposables;
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
                this.BindCommand(ViewModel, vm => vm.CheckVersionCommand, v => v.CheckVersionButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddAccountCommand, v => v.AddAccountButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddAccountsCommand, v => v.AddAccountsButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.LoginButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.LogoutButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.EditAccountCommand, v => v.EditButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAccountCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoginAllCommand, v => v.LoginAllButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LogoutAllCommand, v => v.LogoutAllButton).DisposeWith(d);
            });
        }
    }
}