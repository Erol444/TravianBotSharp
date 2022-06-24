using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
using TTWarsCore;
using WPFUI.Views;

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

            InitializeComponent();

            #region Commands

            this.BindCommand(ViewModel,
                vm => vm.AddAccountCommand,
                v => v.AddAccount
            );
            this.BindCommand(ViewModel,
               vm => vm.AddAccountsCommand,
               v => v.AddAccounts
           );
            this.BindCommand(ViewModel,
               vm => vm.LoginCommand,
               v => v.Login
           );
            this.BindCommand(ViewModel,
               vm => vm.LogoutCommand,
               v => v.Logout
           );
            this.BindCommand(ViewModel,
               vm => vm.EditAccountCommand,
               v => v.Edit
           );
            this.BindCommand(ViewModel,
               vm => vm.DeleteAccountCommand,
               v => v.Delete
           );
            this.BindCommand(ViewModel,
               vm => vm.LoginAllCommand,
               v => v.LoginAll
           );
            this.BindCommand(ViewModel,
               vm => vm.LogoutAllCommand,
               v => v.LogoutAll
           );

            #endregion Commands

            #region Events

            this.Events().Closing.InvokeCommand(this, x => x.ViewModel.ClosingCommand);

            #endregion Events
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}