using DynamicData;
using MainCore;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc;

namespace WPFUI.ViewModels.Tabs
{
    public class AddAccountsViewModel : TabBaseViewModel
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IEventManager _eventManager;

        private readonly WaitingOverlayViewModel _waitingOverlay;

        public AddAccountsViewModel(IDbContextFactory<AppDbContext> contextFactory, WaitingOverlayViewModel waitingWindow, IUseragentManager useragentManager, IEventManager eventManager)
        {
            _contextFactory = contextFactory;
            _waitingOverlay = waitingWindow;
            _useragentManager = useragentManager;
            _eventManager = eventManager;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            UpdateTable = ReactiveCommand.CreateFromTask<string>(UpdateTableTask);

            this.WhenAnyValue(x => x.InputText).InvokeCommand(UpdateTable);
        }

        private async Task SaveTask()
        {
            if (!IsVaildInput()) return;
            _waitingOverlay.ShowCommand.Execute("adding accounts").Subscribe();

            await Task.Run(() =>
            {
                using var context = _contextFactory.CreateDbContext();

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
            });
            Clean();
            _eventManager.OnAccountsUpdate();
            _waitingOverlay.CloseCommand.Execute().Subscribe();
            MessageBox.Show($"Added account to TBS's database", "Success");
        }

        private async Task UpdateTableTask(string input)
        {
            if (!IsActive) return;
            if (string.IsNullOrWhiteSpace(input))
            {
                if (Accounts.Count > 0) Accounts.Clear();
                return;
            }
            var strArr = input.Trim().Split('\n');
            var listTasks = new List<Task<AccountMulti>>();
            foreach (var str in strArr)
            {
                listTasks.Add(Task.Run(() => AccountParser(str)));
            }

            var listResult = await Task.WhenAll(listTasks);
            listResult = listResult.Where(x => x is not null).ToArray();

            Accounts.Clear();
            Accounts.AddRange(listResult);
        }

        private void Clean()
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                InputText = "";
                Accounts.Clear();
            });
        }

        private bool IsVaildInput()
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
                        MessageBox.Show("There is not a number proxy's port.", "Warning");
                        return false;
                    }
                }
            }
            return true;
        }

        private static AccountMulti AccountParser(string input)
        {
            var strAccount = input.Trim().Split(' ');
            Uri url = null;
            if (strAccount.Length > 0)
            {
                if (!Uri.TryCreate(strAccount[0], UriKind.Absolute, out url))
                {
                    return null;
                };
            }
            return strAccount.Length switch
            {
                3 => new AccountMulti()
                {
                    Server = url.AbsoluteUri,
                    Username = strAccount[1],
                    Password = strAccount[2],
                },
                5 => new AccountMulti()
                {
                    Server = url.AbsoluteUri,
                    Username = strAccount[1],
                    Password = strAccount[2],
                    ProxyHost = strAccount[3],
                    ProxyPort = strAccount[4],
                },
                7 => new AccountMulti()
                {
                    Server = url.AbsoluteUri,
                    Username = strAccount[1],
                    Password = strAccount[2],
                    ProxyHost = strAccount[3],
                    ProxyPort = strAccount[4],
                    ProxyUsername = strAccount[5],
                    ProxyPassword = strAccount[6],
                },
                _ => null,
            };
        }

        private string _inputText;

        public string InputText
        {
            get => _inputText;
            set => this.RaiseAndSetIfChanged(ref _inputText, value);
        }

        public ObservableCollection<AccountMulti> Accounts { get; } = new();
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<string, Unit> UpdateTable { get; }
    }
}