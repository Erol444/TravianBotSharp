using MainCore;
using MainCore.Models.Database;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using WPFUI.Interfaces;

namespace WPFUI.ViewModels.Uc
{
    public class AccountTableViewModel : ReactiveObject, ITabPage
    {
        public AccountTableViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = App.GetService<IEventManager>();
            _eventManager.AccountsTableUpdate += OnAccountTableUpdate;

            _isAccountSelected = this.WhenAnyValue(x => x.CurrentAccount).Select(x => x is not null).ToProperty(this, x => x.IsAccountSelected);
        }

        private void OnAccountTableUpdate()
        {
            LoadData();
        }

        public void OnActived()
        {
            LoadData();
        }

        private void LoadData()
        {
            using var context = _contextFactory.CreateDbContext();
            Accounts.Clear();
            foreach (var item in context.Accounts)
            {
                Accounts.Add(item);
            }
        }

        public ObservableCollection<Account> Accounts { get; } = new();

        private Account _currentAccount;

        public Account CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountSelected;

        public bool IsAccountSelected
        {
            get => _isAccountSelected.Value;
        }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
    }
}