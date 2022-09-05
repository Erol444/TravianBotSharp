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
    public class AddAccountViewModel : ReactiveObject
    {
        public AddAccountViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _waitingWindow = App.GetService<WaitingWindow>();
            _databaseEvent = App.GetService<IEventManager>();
            _useragentManager = App.GetService<IUseragentManager>();

            SaveCommand = ReactiveCommand.Create(SaveTask, outputScheduler: RxApp.TaskpoolScheduler);
            CancelCommand = ReactiveCommand.Create(CancelTask);
        }

        private void SaveTask()
        {
            CheckInput();
            _waitingWindow.ViewModel.Show("saving account");

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
                var dbAccess = new MainCore.Models.Database.Access()
                {
                    AccountId = account.Id,
                    Password = access.Password,
                    ProxyHost = access.ProxyHost,
                    ProxyPort = int.Parse(access.ProxyPort ?? "-1"),
                    ProxyUsername = access.ProxyUsername,
                    ProxyPassword = access.ProxyPassword,
                    Useragent = _useragentManager.Get(),
                };
                context.Accesses.Add(dbAccess);
                context.SaveChanges();
                access.Id = dbAccess.Id;
            }
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