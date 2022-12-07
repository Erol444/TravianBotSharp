using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for AddAccountsPage.xaml
    /// </summary>
    public partial class AddAccountsPage : ReactivePage<AddAccountsViewModel>
    {
        public AddAccountsPage()
        {
            ViewModel = Locator.Current.GetService<AddAccountsViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.AddButton).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.AccountsDatagrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.InputText, v => v.AccountsInput.Text).DisposeWith(d);
            });
        }
    }
}