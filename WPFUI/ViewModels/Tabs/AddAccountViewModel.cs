using MainCore.Models.Database;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class AddAccountViewModel : TabBaseViewModel
    {
        public AddAccountViewModel()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            CancelCommand = ReactiveCommand.Create(CancelTask);
        }

        private async Task SaveTask()
        {
            if (!CheckInput()) return;
            _waitingWindow.ViewModel.Show("saving account");
            await Task.Run(() =>
            {
                var context = _contextFactory.CreateDbContext();
                if (context.Accounts.Any(x => x.Username.Equals(Username) && x.Server.Equals(Server)))
                {
                    _waitingWindow.ViewModel.Close();
                    MessageBox.Show("This account was already in TBS", "Warning");
                    return;
                }
                Uri.TryCreate(Server, UriKind.Absolute, out var url);
                var account = new Account()
                {
                    Username = Username,
                    Server = url.AbsoluteUri,
                };

                context.Add(account);
                context.SaveChanges();

                context.AddAccount(account.Id);
                context.SaveChanges();

                foreach (var access in Accessess)
                {
                    context.Accesses.Add(new()
                    {
                        AccountId = account.Id,
                        Password = access.Password,
                        ProxyHost = access.ProxyHost,
                        ProxyPort = int.Parse(access.ProxyPort ?? "-1"),
                        ProxyUsername = access.ProxyUsername,
                        ProxyPassword = access.ProxyPassword,
                        Useragent = _useragentManager.Get(),
                    });
                }
                context.SaveChanges();
            });
            Clean();
            _eventManager.OnAccountsTableUpdate();
            _waitingWindow.ViewModel.Close();
        }

        private void CancelTask()
        {
            Clean();
            TabSelector = TabType.NoAccount;
        }

        private void Clean()
        {
            Server = "";
            Username = "";
            Accessess.Clear();
        }

        private bool CheckInput()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("Username is empty", "Warning");
                return false;
            }
            if (string.IsNullOrWhiteSpace(Server))
            {
                MessageBox.Show("Server is empty", "Warning");
                return false;
            }
            if (!Uri.TryCreate(Server, UriKind.Absolute, out _))
            {
                MessageBox.Show("Server string is invaild", "Warning");
                return false;
            };
            if (Accessess.Count == 0)
            {
                MessageBox.Show("No password in table", "Warning");
                return false;
            }
            foreach (var access in Accessess)
            {
                if (string.IsNullOrWhiteSpace(access.Password))
                {
                    MessageBox.Show("There is empty password.", "Warning");
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(access.ProxyHost))
                {
                    if (string.IsNullOrWhiteSpace(access.ProxyPort))
                    {
                        MessageBox.Show("There is empty proxy's port.", "Warning");
                        return false;
                    }
                    if (!int.TryParse(access.ProxyPort, out _))
                    {
                        MessageBox.Show("There is not a number proxy's port.", "Warning");
                        return false;
                    }
                }
            }
            return true;
        }

        private string _server;

        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }

        private string _username;

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public ObservableCollection<Models.Access> Accessess { get; } = new();

        private TabType _tabSelector;

        public TabType TabSelector
        {
            get => _tabSelector;
            set => this.RaiseAndSetIfChanged(ref _tabSelector, value);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    }
}