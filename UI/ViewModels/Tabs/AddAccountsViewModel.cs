using MainCore;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using UI.Models;
using UI.ViewModels.UserControls;

namespace UI.ViewModels.Tabs
{
    public sealed class AddAccountsViewModel : ViewModelBase
    {
        public AddAccountsViewModel(LoadingOverlayViewModel loadingOverlayViewModel, IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager, IEventManager eventManager)
        {
            _loadingOverlayViewModel = loadingOverlayViewModel;
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _eventManager = eventManager;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);

            this.WhenAnyValue(x => x.InputText).Subscribe(ParseData);
        }

        private async Task SaveTask()
        {
            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "Saving account...";

            var context = _contextFactory.CreateDbContext();
            foreach (var account in Accounts)
            {
                var isExist = await context.Accounts.AnyAsync(x => x.Username.Equals(account.Username) && x.Server.Equals(account.Server));
                if (isExist) continue;

                Uri.TryCreate(account.Server, UriKind.Absolute, out var url);
                var acc = new Account()
                {
                    Username = account.Username,
                    Server = url.AbsoluteUri,
                };

                context.Add(acc);
                await context.SaveChangesAsync();

                context.AddAccount(acc.Id);
                await context.SaveChangesAsync();

                context.Accesses.Add(new()
                {
                    AccountId = acc.Id,
                    Password = account.Password,
                    ProxyHost = account.ProxyHost,
                    ProxyPort = account.ProxyPort,
                    ProxyUsername = account.ProxyUsername,
                    ProxyPassword = account.ProxyPassword,
                    Useragent = _useragentManager.Get(),
                });
            }
            context.SaveChanges();

            _eventManager.OnAccountsUpdate();
            _loadingOverlayViewModel.Unload();

            var messageSuccess = MessageBoxManager.GetMessageBoxStandardWindow("Success", "Accounts are added");
        }

        private void ParseData(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Accounts.Clear();
            foreach (var line in lines)
            {
                var acc = ParseLine(line);
                if (line is null) continue;
                Accounts.Add(acc);
            }
        }

        private static AccountModel ParseLine(string line)
        {
            var data = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length < 3) return null;
            if (!Uri.TryCreate(data[0], UriKind.Absolute, out var url)) return null;

            var account = new AccountModel
            {
                Server = url.AbsoluteUri
            };
            switch (data.Length)
            {
                case 3:
                    {
                        account.Username = data[1];
                        account.Password = data[2];
                        break;
                    }
                case 5:
                    {
                        account.Username = data[1];
                        account.Password = data[2];
                        account.ProxyHost = data[3];
                        if (!int.TryParse(data[4], out var port)) return null;
                        account.ProxyPort = port;
                        break;
                    }
                case 7:
                    {
                        account.Username = data[1];
                        account.Password = data[2];
                        account.ProxyHost = data[3];
                        if (!int.TryParse(data[4], out var port)) return null;
                        account.ProxyPort = port;
                        account.ProxyUsername = data[5];
                        account.ProxyPassword = data[6];
                        break;
                    }
                default:
                    return null;
            }

            return account;
        }

        private string _inputText;

        public string InputText
        {
            get => _inputText;
            set => this.RaiseAndSetIfChanged(ref _inputText, value);
        }

        public ObservableCollection<AccountModel> Accounts { get; } = new();
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IEventManager _eventManager;
    }
}