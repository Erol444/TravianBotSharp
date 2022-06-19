using MainCore.Services;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow(IChromeManager chromeManager)
        {
            ViewModel = new(chromeManager);
            ViewModel.RequestClose += Close;
            ViewModel.RequestHide += Hide;
            InitializeComponent();

            this.WhenActivated(d =>
            {
                #region Commands

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

                #endregion Commands

                #region Events

                this.Events().Closing.InvokeCommand(this, x => x.ViewModel.ClosingCommand);

                #endregion Events
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}