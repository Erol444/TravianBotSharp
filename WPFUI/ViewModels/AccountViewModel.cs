using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using TTWarsCore;
using TTWarsCore.Models;
using WPFUI.Views;

namespace WPFUI.ViewModels
{
    public class AccountViewModel : ReactiveObject
    {
        public AccountViewModel()
        {
            _contextFactory = SetupService.GetService<IDbContextFactory<AppDbContext>>();

            TestCommand = ReactiveCommand.CreateFromTask(TestTask);
            TestAllCommand = ReactiveCommand.CreateFromTask(TestAllTask);
            SaveCommand = ReactiveCommand.Create(SaveTask);
            CancelCommand = ReactiveCommand.Create(CancelTask);
        }

        public void LoadData()
        {
            if (AccountId == -1) return;
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.FirstOrDefault(x => x.Id == AccountId);
            if (account == null) return;
            Username = account.Username;
            Server = account.Server;
            var accesses = context.Accesses.Where(x => x.AccountId == AccountId);
            Accessess.Clear();
            foreach (var item in accesses)
            {
                Accessess.Add(new Models.Access()
                {
                    Password = item.Password,
                    ProxyHost = item.ProxyHost,
                    ProxyPort = item.ProxyPort.ToString(),
                    ProxyUsername = item.ProxyUsername,
                    ProxyPassword = item.ProxyPassword,
                });
            }
        }

        private async Task TestTask()
        {
            await Task.Delay(599);
        }

        private async Task TestAllTask()
        {
            _ = Accessess.Count;
            await Task.Delay(599);
        }

        private void SaveTask()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("Username is empty", "Warning");
                return;
            }
            if (string.IsNullOrWhiteSpace(Server))
            {
                MessageBox.Show("Username is empty", "Warning");
                return;
            }
            if (Accessess.Count == 0)
            {
                MessageBox.Show("No password in table", "Warning");
                return;
            }
            foreach (var access in Accessess)
            {
                if (string.IsNullOrWhiteSpace(access.Password))
                {
                    MessageBox.Show("There is empty password.", "Warning");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(access.ProxyHost))
                {
                    if (string.IsNullOrWhiteSpace(access.ProxyPort))
                    {
                        MessageBox.Show("There is empty proxy's port.", "Warning");
                        return;
                    }
                    if (!int.TryParse(access.ProxyPort, out _))
                    {
                        MessageBox.Show("There is non-numeric proxy's port.", "Warning");
                        return;
                    }
                }
            }

            var context = _contextFactory.CreateDbContext();

            if (AccountId == -1)
            {
                if (context.Accounts.Any(x => x.Username.Equals(Username) && x.Server.Equals(Server)))
                {
                    MessageBox.Show("This account was already in TBS", "Warning");
                    return;
                }

                var account = new Account()
                {
                    Username = Username,
                    Server = Server,
                };

                context.Add(account);
                context.SaveChanges();
                AccountId = account.Id;
            }
            else
            {
                var account = context.Accounts.FirstOrDefault(x => x.Id == AccountId);
                if (account is null) return;

                account.Server = Server;
                account.Username = Username;

                var accesses = context.Accesses.Where(x => x.AccountId == AccountId);

                context.Accesses.RemoveRange(accesses);
                context.SaveChanges();
            }

            foreach (var access in Accessess)
            {
                var accessDb = new Access()
                {
                    AccountId = AccountId,
                    Password = access.Password,
                    ProxyHost = access.ProxyHost,
                    ProxyPort = int.Parse(access.ProxyPort ?? "-1"),
                    ProxyUsername = access.ProxyUsername,
                    ProxyPassword = access.ProxyPassword,
                };
                context.Add(accessDb);
            }
            context.SaveChanges();

            Clean();
        }

        private void CancelTask()
        {
            Clean();
        }

        private void Clean()
        {
            var accountWindow = SetupService.GetService<AccountWindow>();

            if (!accountWindow.Dispatcher.CheckAccess())
            {
                accountWindow.Dispatcher.Invoke(() =>
                {
                    accountWindow.Hide();
                    Server = "";
                    Username = "";
                    Accessess.Clear();
                });
            }
            else
            {
                accountWindow.Hide();
                Server = "";
                Username = "";
                Accessess.Clear();
            }
        }

        private string _server;
        private string _username;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public ObservableCollection<Models.Access> Accessess { get; } = new();

        public int AccountId { get; set; }

        public ReactiveCommand<Unit, Unit> TestCommand { get; }
        public ReactiveCommand<Unit, Unit> TestAllCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    }
}