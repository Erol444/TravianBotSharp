using MainCore;
using MainCore.Services.Interface;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using UI.Models;
using UI.ViewModels.UserControls;

namespace UI.ViewModels.Tabs
{
    public class EditAccountViewModel : ActivatableViewModelBase
    {
        public EditAccountViewModel(LoadingOverlayViewModel loadingOverlayViewModel, IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager, AccountViewModel accountViewModel) : base()
        {
            _loadingOverlayViewModel = loadingOverlayViewModel;
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _accountViewModel = accountViewModel;

            _accountViewModel.AccountChanged += OnAccountChanged;
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
        }

        protected override void OnActived(CompositeDisposable disposable)
        {
            base.OnActived(disposable);
            var accountId = _accountViewModel.Account.Id;
            RxApp.MainThreadScheduler.Schedule(async () => await LoadTask(accountId));
        }

        private void OnAccountChanged(int accountId)
        {
            if (!IsActive) return;
            RxApp.MainThreadScheduler.Schedule(async () => await LoadTask(accountId));
        }

        private async Task LoadTask(int accountId)
        {
            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "loading account...";

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(accountId);
            if (account is null) return;

            Account.Username = account.Username;
            Account.Server = account.Server;

            var access = await context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);

            Account.Password = access.Password;
            Account.ProxyHost = access.ProxyHost;
            Account.ProxyPort = access.ProxyPort;
            Account.ProxyUsername = access.ProxyUsername;
            Account.ProxyPassword = access.ProxyPassword;

            _loadingOverlayViewModel.Unload();
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
            var accountId = _accountViewModel.Account.Id;
            var access = await context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);

            access.Password = Account.Password;
            access.ProxyHost = Account.ProxyHost;
            access.ProxyPort = Account.ProxyPort;
            access.ProxyUsername = Account.ProxyUsername;
            access.ProxyPassword = Account.ProxyPassword;
            access.Useragent = _useragentManager.Get();

            context.Update(access);
            context.SaveChanges();

            _loadingOverlayViewModel.Unload();

            var messageSuccess = MessageBoxManager.GetMessageBoxStandardWindow("Success", "Account is saved");
            await messageSuccess.Show();
        }

        private readonly AccountModel _account = new();

        public AccountModel Account => _account;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;
        private readonly AccountViewModel _accountViewModel;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IUseragentManager _useragentManager;
    }
}