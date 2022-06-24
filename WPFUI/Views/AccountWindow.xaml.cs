using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Reactive.Disposables;
using TTWarsCore;
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

            this.Bind(ViewModel,
                vm => vm.Access,
                v => v.ProxiesDataGrid.DataContext);

            #endregion Data
        }
    }
}