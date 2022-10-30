using Avalonia.Threading;
using MainCore;
using MainCore.Models.Database;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace UI.ViewModels.UserControls
{
    public class AccountTableViewModel : ViewModelBase
    {
        public AccountTableViewModel(IDbContextFactory<AppDbContext> contextFactory, LoadingOverlayViewModel loadingOverlayViewModel, AccountViewModel accountViewModel) : base()
        {
            _contextFactory = contextFactory;
            _accountViewModel = accountViewModel;
            _loadingOverlayViewModel = loadingOverlayViewModel;
            this.WhenAnyValue(vm => vm.CurrentAccount).Subscribe(x => _accountViewModel.Account = x);
            LoadCommand = ReactiveCommand.CreateFromTask(LoadTask);
        }

        public Task LoadData() => LoadTask();

        private async Task LoadTask()
        {
            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "Loading accounts ...";

            await Dispatcher.UIThread.InvokeAsync(Load);

            _loadingOverlayViewModel.Unload();
        }

        private void Load()
        {
            using var context = _contextFactory.CreateDbContext();
            Accounts.Clear();
            if (context.Accounts.Any())
            {
                foreach (var item in context.Accounts)
                {
                    Accounts.Add(item);
                }
            }
        }

        private Account _currentAccount;

        public Account CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set => this.RaiseAndSetIfChanged(ref _currentIndex, value);
        }

        public ObservableCollection<Account> Accounts { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;
        private readonly AccountViewModel _accountViewModel;
    }
}