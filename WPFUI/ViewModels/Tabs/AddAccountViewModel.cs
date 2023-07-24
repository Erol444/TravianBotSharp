using MainCore;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc;

namespace WPFUI.ViewModels.Tabs
{
    public class AddAccountViewModel : TabBaseViewModel
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IEventManager _eventManager;

        private readonly WaitingOverlayViewModel _waitingOverlay;

        public AddAccountViewModel(WaitingOverlayViewModel waitingWindow, IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager, IEventManager eventManager)
        {
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _eventManager = eventManager;
            _waitingOverlay = waitingWindow;
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
        }

        private async Task SaveTask()
        {
            if (!CheckInput()) return;
            _waitingOverlay.Show("saving account");
            await Task.Run(() =>
            {
                var context = _contextFactory.CreateDbContext();
                if (context.Accounts.Any(x => x.Username.Equals(Username) && x.Server.Equals(Server)))
                {
                    _waitingOverlay.Close();
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
            _eventManager.OnAccountsUpdate();
            _waitingOverlay.Close();
        }

        private void Clean()
        {
            Observable.Start(() =>
            {
                Server = "";
                Username = "";
                Accessess.Clear();
            }, RxApp.MainThreadScheduler);
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