using MainCore;
using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Services;
using MainCore.Tasks.Misc;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using WPFUI.Views;

namespace WPFUI
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            _chromeManager = SetupService.GetService<IChromeManager>();
            _contextFactory = SetupService.GetService<IDbContextFactory<AppDbContext>>();
            _databaseEvent = SetupService.GetService<IDatabaseEvent>();
            _databaseEvent.AccountsTableUpdate += LoadData;
            _databaseEvent.AccountStatusUpdate += OnAccountUpdate;
            _taskManager = SetupService.GetService<ITaskManager>();
            _logManager = SetupService.GetService<ILogManager>();

            _accountWindow = SetupService.GetService<AccountWindow>();
            _accountsWindow = SetupService.GetService<AccountsWindow>();
            _waitingWindow = SetupService.GetService<WaitingWindow>();
            _versionWindow = SetupService.GetService<VersionWindow>();

            CheckVersionCommand = ReactiveCommand.Create(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.Create(EditAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
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
                using var context = _contextFactory.CreateDbContext();
                var access = context.Accesses.Where(x => x.AccountId == CurrentAccount.Id).OrderBy(x => x.LastUsed).FirstOrDefault();
                var chromeBrowser = _chromeManager.Get(CurrentAccount.Id);
                chromeBrowser.Setup(access);
                _logManager.AddAccount(CurrentAccount.Id);
                _taskManager.Add(CurrentAccount.Id, new LoginTask(CurrentAccount.Id, _contextFactory, chromeBrowser, _taskManager, _databaseEvent, _logManager));
            });
            await Task.Delay(5000);

            _taskManager.UpdateAccountStatus(CurrentAccount.Id, AccountStatus.Online);
        }

        private async Task LogoutTask()
        {
            _taskManager.UpdateAccountStatus(CurrentAccount.Id, AccountStatus.Stopping);
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

        private async Task ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            _waitingWindow.ViewModel.Text = "saving data";
            _waitingWindow.Show();
            await Task.Delay(2000);
            var mainWindow = SetupService.GetService<MainWindow>();
            mainWindow.Hide();
            await Task.Run(SetupService.Shutdown);
            _closed = true;
            _waitingWindow.Close();
            mainWindow.Close();
        }

        private void DeleteAccount(int index)
        {
            using var context = _contextFactory.CreateDbContext();

            var accesses = context.Accesses.Where(x => x.AccountId == index);
            context.Accesses.RemoveRange(accesses);

            var account = context.Accounts.FirstOrDefault(x => x.Id == index);
            context.Accounts.Remove(account);
            context.SaveChanges();
        }

        private void OnAccountUpdate()
        {
            if (CurrentAccount is not null)
            {
                IsAccountRunning = _taskManager.GetAccountStatus(CurrentAccount.Id) == AccountStatus.Online;
                IsAccountNotRunning = _taskManager.GetAccountStatus(CurrentAccount.Id) == AccountStatus.Offline;
            }
        }

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IDatabaseEvent _databaseEvent;
        private readonly ITaskManager _taskManager;
        private readonly ILogManager _logManager;

        private readonly AccountWindow _accountWindow;
        private readonly AccountsWindow _accountsWindow;
        private readonly WaitingWindow _waitingWindow;
        private readonly VersionWindow _versionWindow;

        private bool _closed = false;
        private bool _isAccountSelected = false;
        private bool _isAccountNotSelected = true;

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
                        IsAccountNotSelected = true;
                        IsAccountSelected = false;
                        var mainWindow = SetupService.GetService<MainWindow>();
                        mainWindow.NoAccountTab.IsSelected = true;
                        _databaseEvent.OnAccountSelected(-1);
                    }
                    else
                    {
                        IsAccountNotSelected = false;
                        IsAccountSelected = true;
                        if (temp is null)
                        {
                            var mainWindow = SetupService.GetService<MainWindow>();
                            mainWindow.GeneralTab.IsSelected = true;
                        }
                        _databaseEvent.OnAccountSelected(value.Id);
                    }
                    _databaseEvent.OnAccountStatusUpdate();
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
            set => this.RaiseAndSetIfChanged(ref _isAccountSelected, value);
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
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginAllCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutAllCommand { get; }
        public ReactiveCommand<CancelEventArgs, Unit> ClosingCommand { get; }
    }
}