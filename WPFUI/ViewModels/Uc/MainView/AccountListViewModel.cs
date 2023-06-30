using DynamicData;
using DynamicData.Kernel;
using MainCore;
using MainCore.Enums;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Media;
using WPFUI.Models;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class AccountListViewModel : ActivatableViewModelBase
    {
        private readonly SelectedItemStore _selectedItemStore;
        private readonly AccountNavigationStore _accountNavigationStore;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;

        public AccountListViewModel(SelectedItemStore selectedItemStore, IDbContextFactory<AppDbContext> contextFactory, IEventManager eventManager, AccountNavigationStore accountNavigationStore)
        {
            _selectedItemStore = selectedItemStore;
            _contextFactory = contextFactory;
            _accountNavigationStore = accountNavigationStore;
            _eventManager = eventManager;

            _eventManager.AccountsTableUpdate += OnAccountsTableUpdate;
            _eventManager.AccountStatusUpdate += OnAccountStatusUpdate;

            this.WhenAnyValue(x => x.CurrentAccount).BindTo(_selectedItemStore, vm => vm.Account);

            this.WhenAnyValue(x => x.CurrentAccount).WhereNotNull().Subscribe(_ => _accountNavigationStore.SetTab(TabType.Normal));
        }

        private void OnAccountStatusUpdate(int accountId, AccountStatus status)
        {
            if (!IsActive) return;

            var account = Accounts.FirstOrDefault(x => x.Id == accountId);
            if (account is null) return;

            switch (status)
            {
                case AccountStatus.Offline:
                    account.Color = Color.FromRgb(0, 0, 0); // black
                    break;

                case AccountStatus.Starting:
                    account.Color = Color.FromRgb(255, 165, 0); // orange
                    break;

                case AccountStatus.Online:
                    account.Color = Color.FromRgb(0, 255, 0); // green
                    break;

                case AccountStatus.Pausing:
                    account.Color = Color.FromRgb(255, 165, 0); // orange
                    break;

                case AccountStatus.Paused:
                    account.Color = Color.FromRgb(255, 0, 0); // red
                    break;

                case AccountStatus.Stopping:
                    account.Color = Color.FromRgb(255, 165, 0); // orange
                    break;
            }
        }

        private void OnAccountsTableUpdate()
        {
            if (!IsActive) return;
            LoadData();
        }

        protected override void OnActive()
        {
            LoadData();
        }

        private void LoadData()
        {
            using var context = _contextFactory.CreateDbContext();
            var accounts = context.Accounts
                .AsList()
                .Select(x => new ListBoxItem(x.Id, x.Username, x.Server))
                .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Accounts.Clear();
                Accounts.AddRange(accounts);

                if (accounts.Any())
                {
                    CurrentAccount = Accounts[0];
                    _accountNavigationStore.SetTab(TabType.Normal);
                }
                else
                {
                    CurrentAccount = null;
                    _accountNavigationStore.SetTab(TabType.NoAccount);
                }
            });
        }

        public ObservableCollection<ListBoxItem> Accounts { get; } = new();

        private ListBoxItem _currentAccount;

        public ListBoxItem CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }
    }
}