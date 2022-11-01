using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class ButtonsPanelUc : ReactiveUserControl<ButtonsPanelViewModel>
    {
        public ButtonsPanelUc()
        {
            ViewModel = Locator.Current.GetService<ButtonsPanelViewModel>();
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