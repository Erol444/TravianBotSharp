using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for EditAccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : ReactiveWindow<AccountViewModel>
    {
        public AccountWindow()
        {
            ViewModel = new();

            InitializeComponent();

            #region Commands

            this.BindCommand(ViewModel,
                vm => vm.TestAllCommand,
                v => v.TestAllProxiesButton
            );

            this.BindCommand(ViewModel,
                vm => vm.SaveCommand,
                v => v.SaveButton
            );

            this.BindCommand(ViewModel,
                vm => vm.CancelCommand,
                v => v.CancelButton
            );

            #endregion Commands

            #region Data

            this.Bind(ViewModel,
                vm => vm.Server,
                v => v.ServerTextBox.Text);

            this.Bind(ViewModel,
                vm => vm.Username,
                v => v.UsernameTextBox.Text);

            this.OneWayBind(ViewModel,
                vm => vm.Accessess,
                v => v.ProxiesDataGrid.ItemsSource);

            #endregion Data
        }
    }
}