using MainCore;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using UI.Models;
using UI.ViewModels.UserControls;

namespace UI.ViewModels.Tabs
{
    public class AddAccountViewModel : ViewModelBase
    {
        public AddAccountViewModel(LoadingOverlayViewModel loadingOverlayViewModel, IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager, IEventManager eventManager)
        {
            _loadingOverlayViewModel = loadingOverlayViewModel;
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _eventManager = eventManager;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
        }

        private async Task SaveTask()
        {
            var result = Account.IsValid();
            if (result.IsFailed)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error", result.Reasons[0].Message);
                await messageBoxStandardWindow.Show();
                return;
            }

            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "Saving account...";

            var context = _contextFactory.CreateDbContext();
            var isExist = await context.Accounts.AnyAsync(x => x.Username.Equals(Account.Username) && x.Server.Equals(Account.Server));
            if (isExist)
            {
                _loadingOverlayViewModel.Unload();
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error", "Account is already in database");
                await messageBoxStandardWindow.Show();
                return;
            }

            Uri.TryCreate(Account.Server, UriKind.Absolute, out var url);
            var account = new Account()
            {
                Username = Account.Username,
                Server = url.AbsoluteUri,
            };

            context.Add(account);
            await context.SaveChangesAsync();

            context.AddAccount(account.Id);
            await context.SaveChangesAsync();

            context.Accesses.Add(new()
            {
                AccountId = account.Id,
                Password = Account.Password,
                ProxyHost = Account.ProxyHost,
                ProxyPort = Account.ProxyPort,
                ProxyUsername = Account.ProxyUsername,
                ProxyPassword = Account.ProxyPassword,
                Useragent = _useragentManager.Get(),
            });

            context.SaveChanges();

            _eventManager.OnAccountsUpdate();
            _loadingOverlayViewModel.Unload();

            var messageSuccess = MessageBoxManager.GetMessageBoxStandardWindow("Success", "Account is added");
            await messageSuccess.Show();
            Account = new() { Server = url.AbsoluteUri };
        }

        private AccountModel _account = new();

        public AccountModel Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
        private readonly IEventManager _eventManager;
    }
}