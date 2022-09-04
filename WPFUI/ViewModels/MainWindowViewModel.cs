using MainCore;
using MainCore.Models.Database;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            _chromeManager = App.GetService<IChromeManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _databaseEvent = App.GetService<IEventManager>();
            _taskManager = App.GetService<ITaskManager>();
            _logManager = App.GetService<ILogManager>();
            _timeManager = App.GetService<ITimerManager>();
            _restClientManager = App.GetService<IRestClientManager>();
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
                else ShowNoAccountTab = true;
            });
            this.WhenAnyValue(x => x.ShowNoAccountTab).SubscribeOn(RxApp.TaskpoolScheduler).Subscribe(x =>
            {
                if (x) SetTab(TabType.NoAccount);
            });
            this.WhenAnyValue(x => x.ShowNormalTab).SubscribeOn(RxApp.TaskpoolScheduler).Subscribe(x =>
            {
                if (x) SetTab(TabType.Normal);
            });
            this.WhenAnyValue(x => x.ShowAddAccountTab).SubscribeOn(RxApp.TaskpoolScheduler).Subscribe(x =>
            {
                if (x) SetTab(TabType.AddAccount);
            });
            this.WhenAnyValue(x => x.ShowAddAccountsTab).SubscribeOn(RxApp.TaskpoolScheduler).Subscribe(x =>
            {
                if (x) SetTab(TabType.AddAccounts);
            });
            this.WhenAnyValue(x => x.ShowEditAccountTab).SubscribeOn(RxApp.TaskpoolScheduler).Subscribe(x =>
            {
                if (x) SetTab(TabType.EditAccount);
            });
            this.WhenAnyValue(x => x.TabSelector).SubscribeOn(RxApp.TaskpoolScheduler).Subscribe(x =>
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
            _waitingWindow.ViewModel.Text = "saving data";
            _waitingWindow.Show();
            await Task.Delay(2000);

            _planManager.Save();

            var mainWindow = App.GetService<MainWindow>();
            mainWindow.Hide();
            _closed = true;
            _waitingWindow.Close();
            mainWindow.Close();
        }

        private void SetTab(TabType tab)
        {
            switch (tab)
            {
                case TabType.NoAccount:
                    SelectedNoAccount = true;
                    ShowNormalTab = false;
                    ShowAddAccountTab = false;
                    ShowAddAccountsTab = false;
                    ShowEditAccountTab = false;
                    break;

                case TabType.Normal:
                    SelectedNormal = true;
                    ShowNoAccountTab = false;
                    ShowAddAccountTab = false;
                    ShowAddAccountsTab = false;
                    ShowEditAccountTab = false;
                    break;

                case TabType.AddAccount:
                    SelectedAddAccount = true;
                    ShowNoAccountTab = false;
                    ShowNormalTab = false;
                    ShowAddAccountsTab = false;
                    ShowEditAccountTab = false;
                    break;

                case TabType.AddAccounts:
                    SelectedAddAccounts = true;
                    ShowNoAccountTab = false;
                    ShowNormalTab = false;
                    ShowAddAccountTab = false;
                    ShowEditAccountTab = false;
                    break;

                case TabType.EditAccount:
                    SelectedEditAccount = true;
                    ShowNoAccountTab = false;
                    ShowNormalTab = false;
                    ShowAddAccountTab = false;
                    ShowAddAccountsTab = false;
                    break;
            }
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

        private readonly IPlanManager _planManager;
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _databaseEvent;
        private readonly ITaskManager _taskManager;
        private readonly ILogManager _logManager;
        private readonly ITimerManager _timeManager;
        private readonly IRestClientManager _restClientManager;
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
    }
}