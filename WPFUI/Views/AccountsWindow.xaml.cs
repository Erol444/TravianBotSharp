using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for AddAccountsWindow.xaml
    /// </summary>
    public partial class AccountsWindow : ReactiveWindow<AccountsViewModel>
    {
        public AccountsWindow()
        {
            ViewModel = new();
            InitializeComponent();

            #region Commands

            this.BindCommand(ViewModel,
                vm => vm.SaveCommand,
                v => v.AddButton
            );

            this.BindCommand(ViewModel,
                vm => vm.CancelCommand,
                v => v.CancelButton
            );

            #endregion Commands

            #region Data

            this.Bind(ViewModel,
                vm => vm.InputText,
                v => v.AccountsInput.Text);

            this.OneWayBind(ViewModel,
                vm => vm.Accounts,
                v => v.AccountsDatagrid.ItemsSource);

            #endregion Data
        }
    }
}