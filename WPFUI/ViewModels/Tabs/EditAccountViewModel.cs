using MainCore;
using MainCore.Helper;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.Views;

namespace WPFUI.ViewModels.Tabs
{
    public class EditAccountViewModel : ReactiveObject, ITabPage
    {
        public EditAccountViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _waitingWindow = App.GetService<WaitingWindow>();
            _databaseEvent = App.GetService<IEventManager>();
            _useragentManager = App.GetService<IUseragentManager>();
            _restClientManager = App.GetService<IRestClientManager>();

            TestAllCommand = ReactiveCommand.CreateFromTask(TestAllTask, outputScheduler: RxApp.TaskpoolScheduler);
            SaveCommand = ReactiveCommand.Create(SaveTask, outputScheduler: RxApp.TaskpoolScheduler);
            CancelCommand = ReactiveCommand.Create(CancelTask);

            this.WhenAnyValue(x => x.AccountId).SubscribeOn(RxApp.TaskpoolScheduler).Subscribe(LoadData);
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        public void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.FirstOrDefault(x => x.Id == index);

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

        private async Task TestAllTask()
        {
            var checkTasks = new List<Task<bool>>();
            using var context = _contextFactory.CreateDbContext();
            var accesses = context.Accesses.Where(x => x.AccountId == AccountId);
            foreach (var access in accesses)
            {
                checkTasks.Add(Task.Run(() => AccessHelper.CheckAccess(_restClientManager.Get(new(access)))));
            }

            _waitingWindow.ViewModel.Show("testing proxies");

            var results = await Task.WhenAll(checkTasks);

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                Accessess[i].ProxyStatus = result ? "Working" : "Not working";
            }

            _waitingWindow.ViewModel.Close();
        }

        private void SaveTask()
        {
            if (!CheckInput()) return;
            _waitingWindow.ViewModel.Show("saving account");

            var context = _contextFactory.CreateDbContext();

            var account = context.Accounts.FirstOrDefault(x => x.Id == AccountId);
            if (account is null) return;
            Uri.TryCreate(Server, UriKind.Absolute, out var url);
            account.Server = url.AbsoluteUri;
            account.Username = Username;

            var accesses = context.Accesses.Where(x => x.AccountId == AccountId);

            context.Accesses.RemoveRange(accesses);
            context.SaveChanges();

            foreach (var access in Accessess)
            {
                context.Accesses.Add(new()
                {
                    AccountId = AccountId,
                    Password = access.Password,
                    ProxyHost = access.ProxyHost,
                    ProxyPort = int.Parse(access.ProxyPort ?? "-1"),
                    ProxyUsername = access.ProxyUsername,
                    ProxyPassword = access.ProxyPassword,
                    Useragent = _useragentManager.Get(),
                });
            }
            context.SaveChanges();

            _databaseEvent.OnAccountsTableUpdate();
            Clean();
            _waitingWindow.ViewModel.Close();
        }

        private void CancelTask()
        {
            Clean();
        }

        private void Clean()
        {
            Server = "";
            Username = "";
            Accessess.Clear();
            TabSelector = TabType.NoAccount;
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
                        MessageBox.Show("There is non-numeric proxy's port.", "Warning");
                        return false;
                    }
                }
            }
            return true;
        }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _databaseEvent;
        private readonly IUseragentManager _useragentManager;
        private readonly IRestClientManager _restClientManager;
        private readonly WaitingWindow _waitingWindow;

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

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }

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