using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using TTWarsCore;
using WPFUI.Views;

namespace WPFUI
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            _chromeManager = SetupService.GetService<IChromeManager>();
            _contextFactory = SetupService.GetService<IDbContextFactory<AppDbContext>>();
            _accountWindow = SetupService.GetService<AccountWindow>();
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask);
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask);
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask);
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.CreateFromTask(LogoutAllTask);
            ClosingCommand = ReactiveCommand.CreateFromTask<CancelEventArgs>(ClosingTask);
        }

        private async Task AddAccountTask()
        {
            await Task.Run(() =>
            {
                if (!_accountWindow.Dispatcher.CheckAccess())
                {
                    _accountWindow.Dispatcher.Invoke(() =>
                    {
                        _accountWindow.ViewModel.IsNewAccount = true;
                        _accountWindow.Show();
                    });
                }
                else
                {
                    _accountWindow.ViewModel.IsNewAccount = true;
                    _accountWindow.Show();
                }
            });
        }

        private async Task AddAccountsTask()
        {
            await Task.Yield();
            _chromeManager.Clear();
        }

        private async Task LoginTask()
        {
            await Task.Yield();
            var browser = _chromeManager.Get(0);
            browser.Close();
        }

        private async Task LogoutTask()
        {
            await Task.Yield();
            var browser = _chromeManager.Get(0);
            browser.Setup();
        }

        private async Task LoginAllTask()
        {
            await Task.Yield();
        }

        private async Task LogoutAllTask()
        {
            await Task.Yield();
        }

        private async Task EditAccountTask()
        {
            await Task.Yield();
        }

        private async Task DeleteAccountTask()
        {
            await Task.Yield();
        }

        private async Task ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            var closingWindow = new ClosingWindow();
            var mainWindow = SetupService.GetService<MainWindow>();
            mainWindow.Hide();

            closingWindow.Show();

            await Task.Run(_chromeManager.Clear);
            _closed = true;
            closingWindow.Close();
            mainWindow.Close();
        }

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly AccountWindow _accountWindow;
        private bool _closed = false;

        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginAllCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutAllCommand { get; }
        public ReactiveCommand<CancelEventArgs, Unit> ClosingCommand { get; }
    }
}