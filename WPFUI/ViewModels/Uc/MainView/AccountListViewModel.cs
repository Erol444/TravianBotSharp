using DynamicData;
using MainCore;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class AccountListViewModel : ActivatableViewModelBase
    {
        private readonly SelectorViewModel _selectorViewModel;
        private readonly MainTabPanelViewModel _mainTabPanelViewModel;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;

        public AccountListViewModel()
        {
            _selectorViewModel = Locator.Current.GetService<SelectorViewModel>();
            this.WhenAnyValue(x => x.CurrentAccount).BindTo(this, vm => vm._selectorViewModel.Account);

            _mainTabPanelViewModel = Locator.Current.GetService<MainTabPanelViewModel>();

            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = Locator.Current.GetService<IEventManager>();
            _eventManager.AccountsTableUpdate += EventManager_AccountsTableUpdate; ;

            Active += AccountListViewModel_Active;
        }

        private void EventManager_AccountsTableUpdate()
        {
            LoadData();
        }

        private void AccountListViewModel_Active()
        {
            LoadData();
        }

        private void LoadData()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = context.Accounts.ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Accounts.Clear();
                Accounts.AddRange(accounts);

                if (accounts.Any())
                {
                    if (_mainTabPanelViewModel.Current == TabType.NoAccount)
                    {
                        CurrentAccount = Accounts[0];
                    }
                    else
                    {
                        CurrentAccount = null;
                    }
                }
                else
                {
                    CurrentAccount = null;
                    _mainTabPanelViewModel.SetTab(TabType.NoAccount);
                }
            });
        }

        public ObservableCollection<Account> Accounts { get; } = new();

        private Account _currentAccount;

        public Account CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }
    }
}