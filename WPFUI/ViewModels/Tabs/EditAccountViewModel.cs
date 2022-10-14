using MainCore.Helper;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class EditAccountViewModel : AccountTabBaseViewModel, ITabPage
    {
        public EditAccountViewModel() : base()
        {
            TestAllCommand = ReactiveCommand.CreateFromTask(TestAllTask);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            CancelCommand = ReactiveCommand.Create(CancelTask);
        }

        public bool IsActive { get; set; }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentAccount is not null)
            {
                LoadData(CurrentAccount.Id);
            }
        }

        public void OnDeactived()
        {
            IsActive = false;
        }

        protected override void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(index);
            if (account is null) return;

            Username = account.Username;
            Server = account.Server;

            var accesses = context.Accesses.Where(x => x.AccountId == index);
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

        private async Task TestAllTask()
        {
            if (!CheckInput()) return;

            _waitingWindow.ViewModel.Show("testing proxies");
            await Task.Run(() =>
            {
                for (int i = 0; i < Accessess.Count; i++)
                {
                    var proxyHost = Accessess[i].ProxyHost;
                    var proxyPort = Accessess[i].ProxyPort;
                    var proxyUsername = Accessess[i].ProxyUsername;
                    var proxyPassword = Accessess[i].ProxyPassword;

                    var result = AccessHelper.CheckAccess(_restClientManager.Get(new(proxyHost, string.IsNullOrEmpty(proxyPort) ? -1 : int.Parse(proxyPort), proxyUsername, proxyPassword)));
                    Accessess[i].ProxyStatus = result ? "Working" : "Not working";
                }
            });
            _waitingWindow.ViewModel.Close();
        }

        private async Task SaveTask()
        {
            if (!CheckInput()) return;
            _waitingWindow.ViewModel.Show("saving account");
            await Task.Run(() =>
            {
                var context = _contextFactory.CreateDbContext();
                var accountId = CurrentAccount.Id;
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

            _eventManager.OnAccountsTableUpdate();
            Clean();
            _waitingWindow.ViewModel.Close();
            MessageBox.Show("Account saved successfully");
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

        public ReactiveCommand<Unit, Unit> TestAllCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    }
}