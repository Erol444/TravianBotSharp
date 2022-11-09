using Avalonia.Threading;
using MainCore;
using MainCore.Enums;
using MainCore.Helper;
using MainCore.Services.Interface;
using MainCore.Tasks.Misc;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using UI.Views;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace UI.ViewModels.UserControls
{
    public class ButtonsPanelViewModel : ViewModelBase
    {
        public ButtonsPanelViewModel(AccountViewModel accountViewModel, TabPanelViewModel tabPanelViewModel, ITaskManager taskManager, ILogManager logManager, IDbContextFactory<AppDbContext> contextFactory, IRestClientManager restClientManager, IChromeManager chromeManager, ITimerManager timerManager) : base()
        {
            _accountViewModel = accountViewModel;
            _accountViewModel.StatusChanged += OnStatusChanged;
            _tabPanelViewModel = tabPanelViewModel;
            _taskManager = taskManager;
            _logManager = logManager;
            _contextFactory = contextFactory;
            _restClientManager = restClientManager;
            _chromeManager = chromeManager;
            _timerManager = timerManager;
            CheckVersionCommand = ReactiveCommand.CreateFromTask(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask, this.WhenAnyValue(vm => vm._accountViewModel.IsAccountSelected));
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm._accountViewModel.IsAccountSelected, (a, b) => a && b));

            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm._accountViewModel.IsAccountSelected, (a, b) => a && b));
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask, this.WhenAnyValue(vm => vm.IsAllowLogout, vm => vm._accountViewModel.IsAccountSelected, (a, b) => a && b));
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.CreateFromTask(LogoutAllTask);
        }

        private void OnStatusChanged(AccountStatus status)
        {
            switch (status)
            {
                case AccountStatus.Offline:
                    IsAllowLogin = true;
                    IsAllowLogout = false;
                    break;

                case AccountStatus.Starting:
                    IsAllowLogin = false;
                    IsAllowLogout = false;
                    break;

                case AccountStatus.Online:
                    IsAllowLogin = false;
                    IsAllowLogout = true;
                    break;

                case AccountStatus.Pausing:
                    IsAllowLogin = false;
                    IsAllowLogout = false;
                    break;

                case AccountStatus.Paused:
                    IsAllowLogin = false;
                    IsAllowLogout = true;
                    break;

                case AccountStatus.Stopping:
                    IsAllowLogin = false;
                    IsAllowLogout = false;
                    break;
            }
        }

        private async Task CheckVersionTask()
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var versionWindow = Locator.Current.GetService<VersionWindow>();
                versionWindow.Show();
            });
        }

        private Task AddAccountTask()
        {
            _tabPanelViewModel.SetTab(TabType.AddAccount);
            return Task.CompletedTask;
        }

        private Task AddAccountsTask()
        {
            _tabPanelViewModel.SetTab(TabType.AddAccounts);
            return Task.CompletedTask;
        }

        private async Task LoginTask()
        {
            await Login(_accountViewModel.Account.Id);
        }

        private async Task LogoutTask()
        {
            await Logout(_accountViewModel.Account.Id);
        }

        private Task EditAccountTask()
        {
            _tabPanelViewModel.SetTab(TabType.EditAccount);
            return Task.CompletedTask;
        }

        private Task DeleteAccountTask()
        {
            return Task.CompletedTask;
        }

        private Task LogoutAllTask()
        {
            return Task.CompletedTask;
        }

        private Task LoginAllTask()
        {
            return Task.CompletedTask;
        }

        private async Task Login(int accountId)
        {
            _taskManager.UpdateAccountStatus(accountId, AccountStatus.Starting);
            _logManager.AddAccount(accountId);
            using var context = await _contextFactory.CreateDbContextAsync();
            var access = await context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);
            var account = await context.Accounts.FindAsync(accountId);

            _logManager.Information(accountId, $"Checking proxy [{access.ProxyHost ?? "no proxy"}]");
            var result = AccessHelper.CheckAccess(_restClientManager.Get(new(access)));
            if (result)
            {
                _logManager.Information(accountId, $"Proxy [{access.ProxyHost ?? "no proxy"}] is working");
            }
            else
            {
                _taskManager.UpdateAccountStatus(accountId, AccountStatus.Offline);
                _logManager.Information(accountId, $"Proxy [{access.ProxyHost ?? "no proxy"}] is not working");

                var message = MessageBoxManager.GetMessageBoxStandardWindow("Warning", $"{account.Username} is failed to login. Proxy [{access.ProxyHost ?? "no proxy"}] is not working");
                await message.Show();
                return;
            }

            var chromeBrowser = _chromeManager.Get(accountId);
            var setting = await context.AccountsSettings.FindAsync(accountId);
            try
            {
                chromeBrowser.Setup(access, setting);
                chromeBrowser.Navigate(account.Server);
            }
            catch (Exception ex)
            {
                var message = MessageBoxManager.GetMessageBoxStandardWindow("Warning", $"{ex.Message}");
                await message.Show();
                return;
            }
            _taskManager.Add(accountId, new LoginTask(accountId), true);

            var sleepExist = _taskManager.GetList(accountId).FirstOrDefault(x => x.GetType() == typeof(SleepTask));
            if (sleepExist is null)
            {
                (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);
                var time = TimeSpan.FromMinutes(Random.Shared.Next(min, max));
                _taskManager.Add(accountId, new SleepTask(accountId) { ExecuteAt = DateTime.Now.Add(time) });
            }

            _timerManager.Start(accountId);
            _taskManager.UpdateAccountStatus(accountId, AccountStatus.Paused);
        }

        private async Task Logout(int accountId)
        {
            _taskManager.UpdateAccountStatus(accountId, AccountStatus.Stopping);

            var current = _taskManager.GetCurrentTask(accountId);
            if (current is not null)
            {
                current.Cts.Cancel();
                await Task.Run(() => { while (current.Stage != TaskStage.Waiting) { } });
            }

            _chromeManager.Get(accountId).Close();
            _taskManager.UpdateAccountStatus(accountId, AccountStatus.Offline);
        }

        private bool _isAllowLogout;

        public bool IsAllowLogout
        {
            get => _isAllowLogout;
            set => this.RaiseAndSetIfChanged(ref _isAllowLogout, value);
        }

        private bool _isAllowLogin;

        public bool IsAllowLogin
        {
            get => _isAllowLogin;
            set => this.RaiseAndSetIfChanged(ref _isAllowLogin, value);
        }

        public ReactiveCommand<Unit, Unit> CheckVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginAllCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutAllCommand { get; }

        private readonly AccountViewModel _accountViewModel;
        private readonly TabPanelViewModel _tabPanelViewModel;
        private readonly ITaskManager _taskManager;
        private readonly ILogManager _logManager;
        private readonly IRestClientManager _restClientManager;
        private readonly IChromeManager _chromeManager;
        private readonly ITimerManager _timerManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}