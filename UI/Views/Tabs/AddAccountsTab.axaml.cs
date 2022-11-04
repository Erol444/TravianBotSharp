using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.Tabs;

namespace UI.Views.Tabs
{
    public partial class AddAccountsTab : ReactiveUserControl<AddAccountsViewModel>
    {
        public AddAccountsTab()
        {
            ViewModel = Locator.Current.GetService<AddAccountsViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.AddButton).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.InputText, v => v.Input.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.Accounts.Items).DisposeWith(d);
            });
        }
    }
}