using MainCore;
using MainCore.Services;
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
            _databaseEvent = SetupService.GetService<DatabaseEvent>();
            _databaseEvent.AccountsTableUpdate = LoadData;

            _accountWindow = SetupService.GetService<AccountWindow>();
            _accountsWindow = SetupService.GetService<AccountsWindow>();
            _waitingWindow = SetupService.GetService<WaitingWindow>();

            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.Create(EditAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
            LoginCommand = ReactiveCommand.Create(LoginTask);
            LogoutCommand = ReactiveCommand.Create(LogoutTask);
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
                Accounts.Add(new Models.Account
                {
                    Username = item.Username,
                    Server = item.Server,
                });
            }
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

        private void LoginTask()
        {
            var browser = _chromeManager.Get(0);
            browser.Close();
        }

        private void LogoutTask()
        {
            var browser = _chromeManager.Get(0);
            browser.Setup();
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
            _accountWindow.ViewModel.AccountId = CurrentAccountId;
            _accountWindow.ViewModel.LoadData();
            _accountWindow.Show();
        }

        private async Task DeleteAccountTask()
        {
            _waitingWindow.ViewModel.Text = "saving data";
            _waitingWindow.Show();
            await Task.Run(() => DeleteAccount(CurrentAccountId));
            _databaseEvent.OnAccountsTableUpdate();
            _waitingWindow.Hide();
        }

        private async Task ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            _waitingWindow.ViewModel.Text = "saving data";
            _waitingWindow.Show();
            var mainWindow = SetupService.GetService<MainWindow>();
            await Task.Delay(2000);
            mainWindow.Hide();
            _chromeManager.Clear();
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

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly DatabaseEvent _databaseEvent;

        private readonly AccountWindow _accountWindow;
        private readonly AccountsWindow _accountsWindow;
        private readonly WaitingWindow _waitingWindow;

        private bool _closed = false;
        private bool _accountCache = false;
        private bool _isAccountSelected = false;
        private bool _isAccountNotSelected = true;
        private int _currentAccountId = -1;
        private Models.Account _currentAccount;
        public ObservableCollection<Models.Account> Accounts { get; } = new();

        public Models.Account CurrentAccount
        {
            get => _currentAccount;
            set
            {
                var temp = _currentAccount;
                this.RaiseAndSetIfChanged(ref _currentAccount, value);
                if (_currentAccount != temp)
                {
                    _accountCache = false;
                    this.RaisePropertyChanged(nameof(CurrentAccountId));
                    if (value is null)
                    {
                        IsAccountNotSelected = true;
                        IsAccountSelected = false;
                    }
                    else
                    {
                        IsAccountNotSelected = false;
                        IsAccountSelected = true;
                    }
                }
            }
        }

        public int CurrentAccountId
        {
            get
            {
                if (CurrentAccount is not null)
                {
                    if (!_accountCache)
                    {
                        using var context = _contextFactory.CreateDbContext();
                        _currentAccountId = context.Accounts.Where(x => x.Server.Equals(_currentAccount.Server)).FirstOrDefault(x => x.Username.Equals(_currentAccount.Username))?.Id ?? 0;
                    }
                }
                else
                {
                    _currentAccountId = -1;
                }
                return _currentAccountId;
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