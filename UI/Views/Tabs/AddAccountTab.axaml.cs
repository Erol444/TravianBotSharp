using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.Tabs;

namespace UI.Views.Tabs
{
    public partial class AddAccountTab : ReactiveUserControl<AddAccountViewModel>
    {
        public AddAccountTab()
        {
            ViewModel = Locator.Current.GetService<AddAccountViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Account.Username, v => v.Username.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Account.Server, v => v.Server.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Account.Password, v => v.Password.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Account.ProxyHost, v => v.ProxyHost.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Account.ProxyPort, v => v.ProxyPort.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Account.ProxyUsername, v => v.ProxyUsername.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Account.ProxyPassword, v => v.ProxyPassword.Text).DisposeWith(d);
            });
        }
    }
}