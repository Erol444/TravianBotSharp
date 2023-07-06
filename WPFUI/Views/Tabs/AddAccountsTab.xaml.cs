using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class AddAccountsTabBase : ReactiveUserControl<AddAccountsViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for AddAccountsPage.xaml
    /// </summary>
    public partial class AddAccountsTab : AddAccountsTabBase
    {
        public AddAccountsTab()
        {
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