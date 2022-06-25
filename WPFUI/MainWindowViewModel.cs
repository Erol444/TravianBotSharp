using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
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

            var accountAvailable = this.WhenAnyValue(vm => vm.CurrentAccount, vm => vm.CurrentAccount, (currentAccount, b) => currentAccount is not null);

            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.Create(EditAccountTask, accountAvailable);
            DeleteAccountCommand = ReactiveCommand.Create(DeleteAccountTask, accountAvailable);
            LoginCommand = ReactiveCommand.Create(LoginTask);
            LogoutCommand = ReactiveCommand.Create(LogoutTask);
            LoginAllCommand = ReactiveCommand.Create(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.Create(LogoutAllTask);
            ClosingCommand = ReactiveCommand.Create<CancelEventArgs>(ClosingTask);
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
            _accountWindow.ShowDialog();
            LoadData();
        }

        private void AddAccountsTask()
        {
            LoadData();
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

        private void LoginAllTask()
        {
        }

        private void LogoutAllTask()
        {
        }

        private void EditAccountTask()
        {
            _accountWindow.ViewModel.AccountId = CurrentAccountId;
            _accountWindow.ViewModel.LoadData();
            _accountWindow.ShowDialog();
            LoadData();
        }

        private void DeleteAccountTask()
        {
            DeleteAccount(CurrentAccountId);
            LoadData();
        }

        private void ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            var closingWindow = new ClosingWindow();
            var mainWindow = SetupService.GetService<MainWindow>();
            mainWindow.Hide();

            closingWindow.Show();

            _chromeManager.Clear();
            _closed = true;
            closingWindow.Close();
            mainWindow.Close();
        }

        private void DeleteAccount(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.FirstOrDefault(x => x.Id == index);
            if (account is null) return;
            context.Accounts.Remove(account);
            context.SaveChanges();
        }

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly AccountWindow _accountWindow;
        private bool _closed = false;
        private bool _accountCache = false;
        private int _currentAccountId = -1;
        private Models.Account _currentAccount;
        public ObservableCollection<Models.Account> Accounts { get; } = new();

        public Models.Account CurrentAccount
        {
            get => _currentAccount;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentAccount, value);
                if (_currentAccount != value)
                {
                    _accountCache = false;
                    this.RaisePropertyChanged(nameof(CurrentAccountId));
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