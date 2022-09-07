using MainCore;
using MainCore.Enums;
using MainCore.Helper;
using MainCore.Services;
using MainCore.Tasks.Misc;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.Views;
using Access = MainCore.Models.Database.Access;

namespace WPFUI.ViewModels.Uc
{
    public class ButtonPanelViewModel : ReactiveObject, IMainTabPage
    {
        public ButtonPanelViewModel()
        {
            _waitingWindow = App.GetService<WaitingWindow>();
            _versionWindow = new VersionWindow();
            _chromeManager = App.GetService<IChromeManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = App.GetService<IEventManager>();
            _eventManager.AccountStatusUpdate += OnAccountUpdate;
            _taskManager = App.GetService<ITaskManager>();
            _logManager = App.GetService<ILogManager>();
            _timeManager = App.GetService<ITimerManager>();
            _restClientManager = App.GetService<IRestClientManager>();

            _isAccountNotSelected = this.WhenAnyValue(x => x.IsAccountSelected).Select(x => !x).ToProperty(this, x => x.IsAccountNotSelected);
            _isAccountNotRunning = this.WhenAnyValue(x => x.IsAccountRunning).Select(x => !x).ToProperty(this, x => x.IsAccountNotRunning);

            CheckVersionCommand = ReactiveCommand.Create(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.Create(EditAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
            DeleteAccountCommand = ReactiveCommand.Create(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));

            LoginCommand = ReactiveCommand.Create(LoginTask, this.WhenAnyValue(vm => vm.IsAccountNotRunning, vm => vm.IsAccountSelected, (a, b) => a && b));
            LogoutCommand = ReactiveCommand.Create(LogoutTask, this.WhenAnyValue(vm => vm.IsAccountRunning, vm => vm.IsAccountSelected, (a, b) => a && b));
            LoginAllCommand = ReactiveCommand.Create(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.Create(LogoutAllTask);
        }

        private void OnAccountUpdate()
        {
            if (IsAccountSelected)
            {
                IsAccountRunning = _taskManager.GetAccountStatus(AccountId) == AccountStatus.Online;
            }
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

        private void LoginTask() => LoginAccount(AccountId);

        private void LogoutTask() => LogoutAccount(AccountId);

        private void LoginAllTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = context.Accounts;
            foreach (var account in accounts)
            {
                LoginAccount(account.Id);
            }
        }

        private void LogoutAllTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = context.Accounts;
            foreach (var account in accounts)
            {
                LogoutAccount(account.Id);
            }
        }

        private void EditAccountTask()
        {
            TabSelector = TabType.EditAccount;
        }

        private void DeleteAccountTask()
        {
            _waitingWindow.ViewModel.Show("saving data");
            DeleteAccount(AccountId);
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
            chromeBrowser.Setup(selectedAccess, setting);

            chromeBrowser.Navigate(account.Server);
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

        public void OnActived()
        {
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

        private bool _isAccountRunning;

        public bool IsAccountRunning
        {
            get => _isAccountRunning;
            set => this.RaiseAndSetIfChanged(ref _isAccountRunning, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountNotRunning;

        public bool IsAccountNotRunning
        {
            get => _isAccountNotRunning.Value;
        }

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }

        private TabType _tabSelector;

        public TabType TabSelector
        {
            get => _tabSelector;
            set => this.RaiseAndSetIfChanged(ref _tabSelector, value);
        }

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;
        private readonly ILogManager _logManager;
        private readonly ITimerManager _timeManager;
        private readonly IRestClientManager _restClientManager;

        private readonly WaitingWindow _waitingWindow;
        private readonly VersionWindow _versionWindow;

        private readonly Random rand = new();
    }
}