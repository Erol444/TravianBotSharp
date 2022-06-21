using MainCore.Services;
using Microsoft.EntityFrameworkCore;
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
        public MainWindowViewModel(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask);
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask);
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask);
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.CreateFromTask(LogoutAllTask);
            ClosingCommand = ReactiveCommand.CreateFromTask<CancelEventArgs>(ClosingTask);

            using var context = _contextFactory.CreateDbContext();
        }

        private async Task AddAccountTask()
        {
            await Task.Run(() =>
            {
                for (var i = 0; i < 2; i++)
                {
                    var browser = _chromeManager.Get(i);
                    browser.Setup();
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
            RequestHide();
            closingWindow.Show();

            await Task.Run(_chromeManager.Clear);
            _closed = true;
            closingWindow.Close();
            RequestClose();
        }

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private bool _closed = false;

        public event Action RequestClose;

        public event Action RequestHide;

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