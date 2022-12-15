using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class EditAccountViewModel : AccountTabBaseViewModel
    {
        public EditAccountViewModel()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(accountId);

            var accesses = context.Accesses
                .Where(x => x.AccountId == accountId)
                .Select(item => new Models.Access()
                {
                    Password = item.Password,
                    ProxyHost = item.ProxyHost,
                    ProxyPort = item.ProxyPort.ToString(),
                    ProxyUsername = item.ProxyUsername,
                    ProxyPassword = item.ProxyPassword,
                })
                .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Username = account.Username;
                Server = account.Server;

                Accessess.Clear();
                Accessess.AddRange(accesses);
            });
        }

        private async Task SaveTask()
        {
            if (!CheckInput()) return;
            _waitingWindow.Show("saving account");
            await Task.Run(() =>
            {
                var context = _contextFactory.CreateDbContext();
                var accountId = _selectorViewModel.Account.Id;
                var account = context.Accounts.FirstOrDefault(x => x.Id == accountId);
                if (account is null) return;
                Uri.TryCreate(Server, UriKind.Absolute, out var url);
                account.Server = url.AbsoluteUri;
                account.Username = Username;

                var accesses = context.Accesses.Where(x => x.AccountId == accountId);

                context.Accesses.RemoveRange(accesses);
                context.SaveChanges();

                foreach (var access in Accessess)
                {
                    context.Accesses.Add(new()
                    {
                        AccountId = accountId,
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

            _eventManager.OnAccountsUpdate();
            Clean();
            _waitingWindow.Close();
            MessageBox.Show("Account saved successfully");
        }

        private void Clean()
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Server = "";
                Username = "";
                Accessess.Clear();
            });
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
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    }
}