using MainCore;
using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Database;
using MainCore.Services;
using MainCore.Tasks.Misc;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Views;

namespace WPFUI
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            _chromeManager = App.GetService<IChromeManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _databaseEvent = App.GetService<IEventManager>();
            _databaseEvent.AccountsTableUpdate += LoadData;
            _databaseEvent.AccountStatusUpdate += OnAccountUpdate;
            _taskManager = App.GetService<ITaskManager>();
            _logManager = App.GetService<ILogManager>();
            _timeManager = App.GetService<ITimerManager>();
            _restClientManager = App.GetService<IRestClientManager>();

            _accountWindow = App.GetService<AccountWindow>();
            _accountsWindow = App.GetService<AccountsWindow>();
            _accountSettingsWindow = App.GetService<AccountSettingsWindow>();
            _waitingWindow = App.GetService<WaitingWindow>();
            _versionWindow = App.GetService<VersionWindow>();

            CheckVersionCommand = ReactiveCommand.Create(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.Create(EditAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
            SettingsAccountCommand = ReactiveCommand.Create(SettingsAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));

            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask, this.WhenAnyValue(vm => vm.IsAccountNotRunning, vm => vm.IsAccountSelected, (a, b) => a && b));
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask, this.WhenAnyValue(vm => vm.IsAccountRunning, vm => vm.IsAccountSelected, (a, b) => a && b));
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.Create(LogoutAllTask);
            ClosingCommand = ReactiveCommand.CreateFromTask<CancelEventArgs>(ClosingTask);
        }

        public void LoadData()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = context.Accounts.ToList();
            Accounts.Clear();
            foreach (var item in accounts)
            {
                Accounts.Add(item);
            }
        }

        private void CheckVersionTask()
        {
            _versionWindow.Show();
        }

        private void AddAccountTask()
        {
            _accountWindow.ViewModel.AccountId = -1;
            _accountWindow.Show();
        }

        private void AddAccountsTask()
        {
            _accountsWindow.Show();
        }

        private async Task LoginTask()
        {
            _taskManager.UpdateAccountStatus(CurrentAccount.Id, AccountStatus.Starting);
            await Task.Run(() =>
            {
                _logManager.AddAccount(CurrentAccount.Id);
                using var context = _contextFactory.CreateDbContext();
                var accesses = context.Accesses.Where(x => x.AccountId == CurrentAccount.Id).OrderBy(x => x.LastUsed);
                Access selectedAccess = null;
                foreach (var access in accesses)
                {
                    if (string.IsNullOrEmpty(access.ProxyHost))
                    {
                        selectedAccess = access;
                        break;
                    }

                    _logManager.Information(CurrentAccount.Id, $"Checking proxy {access.ProxyHost}");
                    var result = AccessHelper.CheckAccess(_restClientManager.Get(access.Id), access.ProxyHost);
                    if (result)
                    {
                        _logManager.Information(CurrentAccount.Id, $"Proxy {access.ProxyHost} is working");
                        selectedAccess = access;
                        access.LastUsed = DateTime.Now;
                        context.SaveChanges();
                        break;
                    }
                    else
                    {
                        _logManager.Information(CurrentAccount.Id, $"Proxy {access.ProxyHost} is not working");
                    }
                }

                if (selectedAccess is null)
                {
                    _logManager.Information(CurrentAccount.Id, "All proxy of this account is not working");
                    MessageBox.Show("All proxy of this account is not working");
                    _taskManager.UpdateAccountStatus(CurrentAccount.Id, AccountStatus.Offline);

                    return;
                }

                var chromeBrowser = _chromeManager.Get(CurrentAccount.Id);
                var setting = context.AccountsSettings.Find(CurrentAccount.Id);
                chromeBrowser.Setup(selectedAccess, setting);
                chromeBrowser.Navigate(CurrentAccount.Server);
                _taskManager.Add(CurrentAccount.Id, new LoginTask(CurrentAccount.Id), true);
                (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);

                var time = TimeSpan.FromMinutes(rand.Next(min, max));
                _taskManager.Add(CurrentAccount.Id, new SleepTask(CurrentAccount.Id) { ExecuteAt = DateTime.Now.Add(time) });
                _timeManager.Start(CurrentAccount.Id);
            });
            _taskManager.UpdateAccountStatus(CurrentAccount.Id, AccountStatus.Online);
        }

        private async Task LogoutTask()
        {
            _taskManager.UpdateAccountStatus(CurrentAccount.Id, AccountStatus.Stopping);

            var current = _taskManager.GetCurrentTask(CurrentAccount.Id);
            if (current is not null)
            {
                current.Cts.Cancel();
                await Task.Run(() =>
                {
                    while (current.Stage != TaskStage.Waiting) { }
                });
            }

            await Task.Run(() =>
            {
                _chromeManager.Get(CurrentAccount.Id).Close();
            });
            _taskManager.UpdateAccountStatus(CurrentAccount.Id, AccountStatus.Offline);
        }

        private async Task LoginAllTask()
        {
            await Task.Yield();
        }

        private void LogoutAllTask()
        {
        }

        private void EditAccountTask()
        {
            _accountWindow.ViewModel.AccountId = CurrentAccount.Id;
            _accountWindow.ViewModel.LoadData();
            _accountWindow.Show();
        }

        private async Task DeleteAccountTask()
        {
            _waitingWindow.ViewModel.Text = "saving data";
            _waitingWindow.Show();
            await Task.Run(() => DeleteAccount(CurrentAccount.Id));
            _databaseEvent.OnAccountsTableUpdate();
            _waitingWindow.Hide();
        }

        private void SettingsAccountTask()
        {
            _accountSettingsWindow.ViewModel.LoadData(CurrentAccount.Id);
            _accountSettingsWindow.Show();
        }

        private async Task ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            _waitingWindow.ViewModel.Text = "saving data";
            _waitingWindow.Show();
            await Task.Delay(2000);

            var planManager = App.GetService<IPlanManager>();
            planManager.Save();

            var mainWindow = App.GetService<MainWindow>();
            mainWindow.Hide();
            _closed = true;
            _waitingWindow.Close();
            mainWindow.Close();
        }

        private void DeleteAccount(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            context.DeleteAccount(index);
            context.SaveChanges();
        }

        private void OnAccountUpdate()
        {
            if (CurrentAccount is not null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    IsAccountRunning = _taskManager.GetAccountStatus(CurrentAccount.Id) == AccountStatus.Online;
                    IsAccountNotRunning = _taskManager.GetAccountStatus(CurrentAccount.Id) == AccountStatus.Offline;
                });
            }
        }

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _databaseEvent;
        private readonly ITaskManager _taskManager;
        private readonly ILogManager _logManager;
        private readonly ITimerManager _timeManager;
        private readonly IRestClientManager _restClientManager;

        private readonly AccountWindow _accountWindow;
        private readonly AccountsWindow _accountsWindow;
        private readonly AccountSettingsWindow _accountSettingsWindow;
        private readonly WaitingWindow _waitingWindow;
        private readonly VersionWindow _versionWindow;

        private bool _closed = false;
        private bool _isAccountSelected = false;
        private bool _isAccountNotSelected = true;
        private readonly Random rand = new();

        private Account _currentAccount;
        public ObservableCollection<Account> Accounts { get; } = new();

        public Account CurrentAccount
        {
            get => _currentAccount;
            set
            {
                var temp = _currentAccount;
                this.RaiseAndSetIfChanged(ref _currentAccount, value);

                if (_currentAccount != temp)
                {
                    if (value is null)
                    {
                        IsAccountSelected = false;
                    }
                    else
                    {
                        IsAccountSelected = true;
                        OnAccountUpdate();
                    }
                }
            }
        }

        private bool _isAccountRunning;

        public bool IsAccountRunning
        {
            get => _isAccountRunning;
            set
            {
                this.RaiseAndSetIfChanged(ref _isAccountRunning, value);
            }
        }

        private bool _isAccountNotRunning;

        public bool IsAccountNotRunning
        {
            get => _isAccountNotRunning;
            set
            {
                this.RaiseAndSetIfChanged(ref _isAccountNotRunning, value);
            }
        }

        public bool IsAccountSelected
        {
            get => _isAccountSelected;
            set
            {
                this.RaiseAndSetIfChanged(ref _isAccountSelected, value);
                IsAccountNotSelected = !value;
            }
        }

        public bool IsAccountNotSelected
        {
            get => _isAccountNotSelected;
            set => this.RaiseAndSetIfChanged(ref _isAccountNotSelected, value);
        }

        public ReactiveCommand<Unit, Unit> CheckVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> SettingsAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginAllCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutAllCommand { get; }
        public ReactiveCommand<CancelEventArgs, Unit> ClosingCommand { get; }
    }
}