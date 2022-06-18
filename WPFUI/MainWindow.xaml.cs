using ReactiveUI;
using System.Reactive.Disposables;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            ViewModel = new();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel,
                    vm => vm.AddAccountCommand,
                    v => v.AddAccount
                ).DisposeWith(d);
                this.BindCommand(ViewModel,
                   vm => vm.AddAccountsCommand,
                   v => v.AddAccounts
               ).DisposeWith(d);
                this.BindCommand(ViewModel,
                   vm => vm.LoginCommand,
                   v => v.Login
               ).DisposeWith(d);
                this.BindCommand(ViewModel,
                   vm => vm.LogoutCommand,
                   v => v.Logout
               ).DisposeWith(d);
                this.BindCommand(ViewModel,
                   vm => vm.EditAccountCommand,
                   v => v.Edit
               ).DisposeWith(d);
                this.BindCommand(ViewModel,
                   vm => vm.DeleteAccountCommand,
                   v => v.Delete
               ).DisposeWith(d);
                this.BindCommand(ViewModel,
                   vm => vm.LoginAllCommand,
                   v => v.LoginAll
               ).DisposeWith(d);
                this.BindCommand(ViewModel,
                   vm => vm.LogoutAllCommand,
                   v => v.LogoutAll
               ).DisposeWith(d);
            });
            InitializeComponent();
        }
    }
}