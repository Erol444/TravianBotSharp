using MainCore;
using MainCore.Models.Database;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Views;

namespace WPFUI.ViewModels
{
    public class AccountsViewModel : ReactiveObject
    {
        public AccountsViewModel()
        {
            _contextFactory = SetupService.GetService<IDbContextFactory<AppDbContext>>();
            _waitingWindow = SetupService.GetService<WaitingWindow>();
            _databaseEvent = SetupService.GetService<IDatabaseEvent>();
            _useragentManager = SetupService.GetService<IUseragentManager>();

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            CancelCommand = ReactiveCommand.Create(CancelTask);
        }

        private async Task SaveTask()
        {
            if (Accounts.Count == 0)
            {
                MessageBox.Show("No account added", "Warning");
                return;
            }
            foreach (var acc in Accounts)
            {
                if (string.IsNullOrWhiteSpace(acc.Password))
                {
                    MessageBox.Show("There is empty password.", "Warning");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(acc.ProxyHost))
                {
                    if (string.IsNullOrWhiteSpace(acc.ProxyPort))
                    {
                        MessageBox.Show("There is empty proxy's port.", "Warning");
                        return;
                    }
                    if (!int.TryParse(acc.ProxyPort, out _))
                    {
                        MessageBox.Show("There is non-numeric proxy's port.", "Warning");
                        return;
                    }
                }
            }
            _waitingWindow.ViewModel.Text = "adding accounts";
            Hide();
            _waitingWindow.Show();

            await Task.Run(() =>
            {
                var context = _contextFactory.CreateDbContext();

                foreach (var acc in Accounts)
                {
                    if (context.Accounts.Any(x => x.Username.Equals(acc.Username) && x.Server.Equals(acc.Server)))
                    {
                        MessageBox.Show($"Account {acc.Username} - {acc.Server} was already in TBS", "Warning");
                        Show();
                        return;
                    }
                    var account = new Account()
                    {
                        Username = acc.Username,
                        Server = acc.Server,
                    };
                    context.Add(account);
                    context.SaveChanges();

                    var accessDb = new Access()
                    {
                        AccountId = account.Id,
                        Password = acc.Password,
                        ProxyHost = acc.ProxyHost,
                        ProxyPort = int.Parse(acc.ProxyPort ?? "-1"),
                        ProxyUsername = acc.ProxyUsername,
                        ProxyPassword = acc.ProxyPassword,
                        Useragent = _useragentManager.Get(),
                    };
                    context.Add(accessDb);
                }
                context.SaveChanges();
            });

            Clean();

            _databaseEvent.OnAccountsTableUpdate();
            _waitingWindow.Hide();
        }

        private void CancelTask()
        {
            Hide();
            Clean();
        }

        private void Clean()
        {
            InputText = "";
            Accounts.Clear();
        }

        private void Hide()
        {
            var accountsWindow = SetupService.GetService<AccountsWindow>();
            accountsWindow.Dispatcher.Invoke(accountsWindow.Hide);
        }

        private void Show()
        {
            var accountsWindow = SetupService.GetService<AccountsWindow>();
            accountsWindow.Dispatcher.Invoke(accountsWindow.Show);
        }

        private string _inputText;

        public string InputText
        {
            get => _inputText;
            set
            {
                this.RaiseAndSetIfChanged(ref _inputText, value);
                if (!_inputText.Equals(value.Trim()))
                {
                    var strArr = value.Trim().Split('\n');
                    Accounts.Clear();
                    foreach (var str in strArr)
                    {
                        var strAccount = str.Trim().Split(' ');

                        switch (strAccount.Length)
                        {
                            case 3:
                                Accounts.Add(new Models.AccountMulti()
                                {
                                    Server = strAccount[0],
                                    Username = strAccount[1],
                                    Password = strAccount[2],
                                });
                                break;

                            case 5:
                                Accounts.Add(new Models.AccountMulti()
                                {
                                    Server = strAccount[0],
                                    Username = strAccount[1],
                                    Password = strAccount[2],
                                    ProxyHost = strAccount[3],
                                    ProxyPort = strAccount[4],
                                });
                                break;

                            case 7:
                                Accounts.Add(new Models.AccountMulti()
                                {
                                    Server = strAccount[0],
                                    Username = strAccount[1],
                                    Password = strAccount[2],
                                    ProxyHost = strAccount[3],
                                    ProxyPort = strAccount[4],
                                    ProxyUsername = strAccount[5],
                                    ProxyPassword = strAccount[6],
                                });
                                break;
                        }
                    }
                }
            }
        }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IDatabaseEvent _databaseEvent;
        private readonly IUseragentManager _useragentManager;

        private readonly WaitingWindow _waitingWindow;

        public ObservableCollection<Models.AccountMulti> Accounts { get; } = new();
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    }
}