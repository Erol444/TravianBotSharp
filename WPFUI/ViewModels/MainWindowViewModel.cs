using MainCore;
using MainCore.Models.Database;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.Views;

namespace WPFUI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, ITabPage
    {
        public MainWindowViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _planManager = App.GetService<IPlanManager>();

            _waitingWindow = App.GetService<WaitingWindow>();

            _eventManager = App.GetService<IEventManager>();
            _eventManager.AccountsTableUpdate += OnAccountTableUpdate;

            _isAccountSelected = this.WhenAnyValue(x => x.CurrentAccount).Select(x => x is not null).ToProperty(this, x => x.IsAccountSelected);
            _isAccountNotSelected = this.WhenAnyValue(x => x.CurrentAccount).Select(x => x is null).ToProperty(this, x => x.IsAccountNotSelected);

            ClosingCommand = ReactiveCommand.CreateFromTask<CancelEventArgs>(ClosingTask);

            this.WhenAnyValue(x => x.IsAccountSelected).Subscribe(x =>
            {
                if (x) ShowNormalTab = true;
            });
            this.WhenAnyValue(x => x.ShowNoAccountTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.NoAccount);
            });
            this.WhenAnyValue(x => x.ShowNormalTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.Normal);
            });
            this.WhenAnyValue(x => x.ShowAddAccountTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.AddAccount);
            });
            this.WhenAnyValue(x => x.ShowAddAccountsTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.AddAccounts);
            });
            this.WhenAnyValue(x => x.ShowEditAccountTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.EditAccount);
            });
            this.WhenAnyValue(x => x.TabSelector).Subscribe(x =>
            {
                switch (x)
                {
                    case TabType.NoAccount:
                        ShowNoAccountTab = true;
                        break;

                    case TabType.Normal:
                        ShowNormalTab = true;
                        break;

                    case TabType.AddAccount:
                        ShowAddAccountTab = true;
                        break;

                    case TabType.AddAccounts:
                        ShowAddAccountsTab = true;
                        break;

                    case TabType.EditAccount:
                        ShowEditAccountTab = true;
                        break;
                }
            });
        }

        private async Task ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            _waitingWindow.ViewModel.Show("saving data");
            await Task.Run(() =>
            {
                _planManager.Save();

                var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
                if (Directory.Exists(path)) Directory.Delete(path, true);
            });

            var mainWindow = App.GetService<MainWindow>();
            mainWindow.Hide();
            _closed = true;
            _waitingWindow.ViewModel.Close();
            mainWindow.Close();
        }

        private void SetTab(TabType tab)
        {
            switch (tab)
            {
                case TabType.NoAccount:
                    CurrentIndex = -1;
                    ShowNormalTab = false;
                    ShowAddAccountTab = false;
                    ShowAddAccountsTab = false;
                    ShowEditAccountTab = false;
                    SelectedNoAccount = true;
                    break;

                case TabType.Normal:
                    ShowNoAccountTab = false;
                    ShowAddAccountTab = false;
                    ShowAddAccountsTab = false;
                    ShowEditAccountTab = false;
                    SelectedNormal = true;
                    break;

                case TabType.AddAccount:
                    CurrentIndex = -1;
                    ShowNoAccountTab = false;
                    ShowNormalTab = false;
                    ShowAddAccountsTab = false;
                    ShowEditAccountTab = false;
                    SelectedAddAccount = true;
                    break;

                case TabType.AddAccounts:
                    CurrentIndex = -1;
                    ShowNoAccountTab = false;
                    ShowNormalTab = false;
                    ShowAddAccountTab = false;
                    ShowEditAccountTab = false;
                    SelectedAddAccounts = true;
                    break;

                case TabType.EditAccount:
                    ShowNoAccountTab = false;
                    ShowNormalTab = false;
                    ShowAddAccountTab = false;
                    ShowAddAccountsTab = false;
                    SelectedEditAccount = true;
                    break;
            }
        }

        private void OnAccountTableUpdate()
        {
            LoadData();
        }

        public void OnActived()
        {
            IsActive = true;
            LoadData();
        }

        public void OnDeactived()
        {
            IsActive = false;
        }

        private void LoadData()
        {
            using var context = _contextFactory.CreateDbContext();
            Accounts.Clear();
            foreach (var item in context.Accounts)
            {
                Accounts.Add(item);
            }
            ShowNoAccountTab = true;
        }

        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;

        private readonly WaitingWindow _waitingWindow;

        private bool _closed = false;

        public ObservableCollection<Account> Accounts { get; } = new();

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

        private readonly ObservableAsPropertyHelper<bool> _isAccountSelected;

        public bool IsAccountSelected
        {
            get => _isAccountSelected.Value;
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountNotSelected;

        public bool IsAccountNotSelected
        {
            get => _isAccountNotSelected.Value;
        }

        private bool _showNoAccountTab;

        public bool ShowNoAccountTab
        {
            get => _showNoAccountTab;
            set => this.RaiseAndSetIfChanged(ref _showNoAccountTab, value);
        }

        private bool _showNormalTab;

        public bool ShowNormalTab
        {
            get => _showNormalTab;
            set => this.RaiseAndSetIfChanged(ref _showNormalTab, value);
        }

        private bool _showAddAccountTab;

        public bool ShowAddAccountTab
        {
            get => _showAddAccountTab;
            set => this.RaiseAndSetIfChanged(ref _showAddAccountTab, value);
        }

        private bool _showAddAccountsTab;

        public bool ShowAddAccountsTab
        {
            get => _showAddAccountsTab;
            set => this.RaiseAndSetIfChanged(ref _showAddAccountsTab, value);
        }

        private bool _showEditAccountTab;

        public bool ShowEditAccountTab
        {
            get => _showEditAccountTab;
            set => this.RaiseAndSetIfChanged(ref _showEditAccountTab, value);
        }

        private bool _selectedNoAccount;

        public bool SelectedNoAccount
        {
            get => _selectedNoAccount;
            set => this.RaiseAndSetIfChanged(ref _selectedNoAccount, value);
        }

        private bool _selectedNormal;

        public bool SelectedNormal
        {
            get => _selectedNormal;
            set => this.RaiseAndSetIfChanged(ref _selectedNormal, value);
        }

        private bool _selectedAddAccount;

        public bool SelectedAddAccount
        {
            get => _selectedAddAccount;
            set => this.RaiseAndSetIfChanged(ref _selectedAddAccount, value);
        }

        private bool _selectedAddAccounts;

        public bool SelectedAddAccounts
        {
            get => _selectedAddAccounts;
            set => this.RaiseAndSetIfChanged(ref _selectedAddAccounts, value);
        }

        private bool _selectedEditAccount;

        public bool SelectedEditAccount
        {
            get => _selectedEditAccount;
            set => this.RaiseAndSetIfChanged(ref _selectedEditAccount, value);
        }

        private TabType _tabSelector;

        public TabType TabSelector
        {
            get => _tabSelector;
            set => this.RaiseAndSetIfChanged(ref _tabSelector, value);
        }

        public ReactiveCommand<CancelEventArgs, Unit> ClosingCommand { get; }
        public bool IsActive { get; set; }
    }
}