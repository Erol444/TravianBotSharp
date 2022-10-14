using MainCore;
using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Database;
using MainCore.Services;
using MainCore.Tasks.Misc;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.Views;
using Access = MainCore.Models.Database.Access;
using EventManager = MainCore.Services.EventManager;

namespace WPFUI.ViewModels.Uc
{
    public class ButtonPanelViewModel : ReactiveObject
    {
        public ButtonPanelViewModel()
        {
            _waitingWindow = App.GetService<WaitingWindow>();
            _versionWindow = App.GetService<VersionWindow>();
            _chromeManager = App.GetService<IChromeManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = App.GetService<EventManager>();
            _taskManager = App.GetService<ITaskManager>();
            _logManager = App.GetService<ILogManager>();
            _timeManager = App.GetService<ITimerManager>();
            _restClientManager = App.GetService<IRestClientManager>();

            _eventManager.AccountStatusUpdate += OnAccountUpdate;

            _isAccountNotSelected = this.WhenAnyValue(x => x.IsAccountSelected).Select(x => !x).ToProperty(this, x => x.IsAccountNotSelected);

            CheckVersionCommand = ReactiveCommand.Create(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.Create(EditAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
            DeleteAccountCommand = ReactiveCommand.Create(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm.IsAccountSelected, (a, b) => a && b));

            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm.IsAccountSelected, (a, b) => a && b));
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask, this.WhenAnyValue(vm => vm.IsAllowLogout, vm => vm.IsAccountSelected, (a, b) => a && b));
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.CreateFromTask(LogoutAllTask);

            this.WhenAnyValue(x => x.CurrentAccount).Subscribe((x) =>
            {
                if (x is null) return;
                LoadData(CurrentAccount.Id);
            });
        }

        private void LoadData(int accountId)
        {
            var status = _taskManager.GetAccountStatus(accountId);
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

        private void OnAccountUpdate(int accountId)
        {
            if (CurrentAccount is null) return;
            if (CurrentAccount.Id != accountId) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadData(accountId));
        }

        private void CheckVersionTask()
        {
            _versionWindow.Show();
        }

        private void AddAccountTask()
        {
            TabSelector = TabType.AddAccount;
        }

        private void AddAccountsTask()
        {
            TabSelector = TabType.AddAccounts;
        }

        private Task LoginTask() => Task.Run(() => LoginAccount(CurrentAccount.Id));

        private Task LogoutTask() => Task.Run(() => LogoutAccount(CurrentAccount.Id));

        private async Task LoginAllTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = context.Accounts;
            foreach (var account in accounts)
            {
                await Task.Run(() => LoginAccount(account.Id));
            }
        }

        private async Task LogoutAllTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = context.Accounts;
            foreach (var account in accounts)
            {
                await Task.Run(() => LogoutAccount(account.Id));
            }
        }

        private void EditAccountTask()
        {
            TabSelector = TabType.EditAccount;
        }

        private void DeleteAccountTask()
        {
            _waitingWindow.ViewModel.Show("saving data");
            DeleteAccount(CurrentAccount.Id);
            _eventManager.OnAccountsTableUpdate();
            _waitingWindow.ViewModel.Close();
        }

        private void LoginAccount(int index)
        {
            _taskManager.UpdateAccountStatus(index, AccountStatus.Starting);
            _logManager.AddAccount(index);
            using var context = _contextFactory.CreateDbContext();
            var accesses = context.Accesses.Where(x => x.AccountId == index).OrderBy(x => x.LastUsed);
            Access selectedAccess = null;
            foreach (var access in accesses)
            {
                if (string.IsNullOrEmpty(access.ProxyHost))
                {
                    selectedAccess = access;
                    break;
                }

                _logManager.Information(index, $"Checking proxy {access.ProxyHost}");
                var result = AccessHelper.CheckAccess(_restClientManager.Get(new(access)));
                if (result)
                {
                    _logManager.Information(index, $"Proxy {access.ProxyHost} is working");
                    selectedAccess = access;
                    access.LastUsed = DateTime.Now;
                    context.SaveChanges();
                    break;
                }
                else
                {
                    _logManager.Information(index, $"Proxy {access.ProxyHost} is not working");
                }
            }

            if (selectedAccess is null)
            {
                _taskManager.UpdateAccountStatus(index, AccountStatus.Offline);
                _logManager.Information(index, "All proxy of this account is not working");
                MessageBox.Show("All proxy of this account is not working");
                return;
            }

            var chromeBrowser = _chromeManager.Get(index);
            var setting = context.AccountsSettings.Find(index);
            var account = context.Accounts.Find(index);
            try
            {
                chromeBrowser.Setup(selectedAccess, setting);

                chromeBrowser.Navigate(account.Server);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
            }
            _taskManager.Add(index, new LoginTask(index), true);

            var sleepExist = _taskManager.GetList(index).FirstOrDefault(x => x.GetType() == typeof(SleepTask));
            if (sleepExist is null)
            {
                (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);
                var time = TimeSpan.FromMinutes(rand.Next(min, max));
                _taskManager.Add(index, new SleepTask(index) { ExecuteAt = DateTime.Now.Add(time) });
            }

            _timeManager.Start(index);
            _taskManager.UpdateAccountStatus(index, AccountStatus.Online);
        }

        private void LogoutAccount(int index)
        {
            _taskManager.UpdateAccountStatus(index, AccountStatus.Stopping);

            var current = _taskManager.GetCurrentTask(index);
            if (current is not null)
            {
                current.Cts.Cancel();
                while (current.Stage != TaskStage.Waiting) { }
            }

            _chromeManager.Get(index).Close();
            _taskManager.UpdateAccountStatus(index, AccountStatus.Offline);
        }

        private void DeleteAccount(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            context.DeleteAccount(index);
            context.SaveChanges();
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

        private bool _isAccountSelected = false;

        public bool IsAccountSelected
        {
            get => _isAccountSelected;
            set => this.RaiseAndSetIfChanged(ref _isAccountSelected, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountNotSelected;

        public bool IsAccountNotSelected
        {
            get => _isAccountNotSelected.Value;
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

        private TabType _tabSelector;

        public TabType TabSelector
        {
            get => _tabSelector;
            set => this.RaiseAndSetIfChanged(ref _tabSelector, value);
        }

        private Account _currentAccount;

        public Account CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly EventManager _eventManager;
        private readonly ITaskManager _taskManager;
        private readonly ILogManager _logManager;
        private readonly ITimerManager _timeManager;
        private readonly IRestClientManager _restClientManager;

        private readonly WaitingWindow _waitingWindow;
        private readonly VersionWindow _versionWindow;

        private readonly Random rand = new();
    }
}