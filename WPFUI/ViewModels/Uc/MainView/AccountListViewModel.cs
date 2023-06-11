using DynamicData;
using DynamicData.Kernel;
using MainCore;
using MainCore.Enums;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Media;
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
            _mainTabPanelViewModel = Locator.Current.GetService<MainTabPanelViewModel>();
            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = Locator.Current.GetService<IEventManager>();
            _eventManager.AccountsTableUpdate += EventManager_AccountsTableUpdate;
            _eventManager.AccountStatusUpdate += EventManager_AccountStatusUpdate;

            Active += AccountListViewModel_Active;

            this.WhenAnyValue(x => x.CurrentAccount).BindTo(_selectorViewModel, vm => vm.Account);
            this.WhenAnyValue(x => x.CurrentAccount)
                .Where(x => x is not null)
                .Subscribe(x => _mainTabPanelViewModel.SetTab(TabType.Normal));
        }

        private void EventManager_AccountStatusUpdate(int accountId, AccountStatus status)
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
                    _mainTabPanelViewModel.SetTab(TabType.Normal);
                }
                else
                {
                    CurrentAccount = null;
                    _mainTabPanelViewModel.SetTab(TabType.NoAccount);
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