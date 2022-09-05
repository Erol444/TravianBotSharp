using MainCore;
using MainCore.Models.Database;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using WPFUI.Models;
using WPFUI.Views;

namespace WPFUI.ViewModels.Tabs
{
    public class AddAccountsViewModel : ReactiveObject
    {
        public AddAccountsViewModel()
        {
            _waitingWindow = App.GetService<WaitingWindow>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _databaseEvent = App.GetService<IEventManager>();
            _useragentManager = App.GetService<IUseragentManager>();

            SaveCommand = ReactiveCommand.Create(SaveTask, outputScheduler: RxApp.TaskpoolScheduler);
            CancelCommand = ReactiveCommand.Create(CancelTask);

            this.WhenAnyValue(x => x.InputText).SubscribeOn(RxApp.TaskpoolScheduler).Subscribe(UpdateData);
        }

        private void SaveTask()
        {
            if (!CheckInput()) return;

            _waitingWindow.ViewModel.Show("adding accounts");

            var context = _contextFactory.CreateDbContext();

            foreach (var acc in Accounts)
            {
                if (context.Accounts.Any(x => x.Username.Equals(acc.Username) && x.Server.Equals(acc.Server)))
                {
                    MessageBox.Show($"Account {acc.Username} - {acc.Server} was already in TBS", "Warning");
                    return;
                }
                var account = new Account()
                {
                    Username = acc.Username,
                    Server = acc.Server,
                };
                context.Add(account);
                context.SaveChanges();
                context.AddAccount(account.Id);

                context.Accesses.Add(new()
                {
                    AccountId = account.Id,
                    Password = acc.Password,
                    ProxyHost = acc.ProxyHost,
                    ProxyPort = int.Parse(acc.ProxyPort ?? "-1"),
                    ProxyUsername = acc.ProxyUsername,
                    ProxyPassword = acc.ProxyPassword,
                    Useragent = _useragentManager.Get(),
                });
            }
            context.SaveChanges();

            Clean();
            _databaseEvent.OnAccountsTableUpdate();
            _waitingWindow.ViewModel.Close();
        }

        private void CancelTask()
        {
            Clean();
        }

        private void Clean()
        {
            InputText = "";
            Accounts.Clear();
            TabSelector = TabType.NoAccount;
        }

        private bool CheckInput()
        {
            if (Accounts.Count == 0)
            {
                MessageBox.Show("No account added", "Warning");
                return false;
            }
            foreach (var acc in Accounts)
            {
                if (string.IsNullOrWhiteSpace(acc.Password))
                {
                    MessageBox.Show("There is empty password.", "Warning");
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(acc.ProxyHost))
                {
                    if (string.IsNullOrWhiteSpace(acc.ProxyPort))
                    {
                        MessageBox.Show("There is empty proxy's port.", "Warning");
                        return false;
                    }
                    if (!int.TryParse(acc.ProxyPort, out _))
                    {
                        MessageBox.Show("There is non-numeric proxy's port.", "Warning");
                        return false;
                    }
                }
            }
            return true;
        }

        private void UpdateData(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            var strArr = text.Trim().Split('\n');
            Accounts.Clear();
            foreach (var str in strArr)
            {
                var strAccount = str.Trim().Split(' ');
                Uri url = null;
                if (strAccount.Length > 0)
                {
                    if (!Uri.TryCreate(strAccount[0], UriKind.Absolute, out url))
                    {
                        continue;
                    };
                }
                switch (strAccount.Length)
                {
                    case 3:
                        Accounts.Add(new AccountMulti()
                        {
                            Server = url.AbsoluteUri,
                            Username = strAccount[1],
                            Password = strAccount[2],
                        });
                        break;

                    case 5:
                        Accounts.Add(new AccountMulti()
                        {
                            Server = url.AbsoluteUri,
                            Username = strAccount[1],
                            Password = strAccount[2],
                            ProxyHost = strAccount[3],
                            ProxyPort = strAccount[4],
                        });
                        break;

                    case 7:
                        Accounts.Add(new AccountMulti()
                        {
                            Server = url.AbsoluteUri,
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

        private string _inputText;

        public string InputText
        {
            get => _inputText;
            set => this.RaiseAndSetIfChanged(ref _inputText, value);
        }

        private TabType _tabSelector;

        public TabType TabSelector
        {
            get => _tabSelector;
            set => this.RaiseAndSetIfChanged(ref _tabSelector, value);
        }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _databaseEvent;
        private readonly IUseragentManager _useragentManager;

        private readonly WaitingWindow _waitingWindow;
        public ObservableCollection<AccountMulti> Accounts { get; } = new();
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    }
}