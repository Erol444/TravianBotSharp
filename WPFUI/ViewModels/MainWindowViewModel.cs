using MainCore;
using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            _taskManager = App.GetService<ITaskManager>();

            _waitingWindow = App.GetService<WaitingWindow>();

            _eventManager = App.GetService<EventManager>();
            _eventManager.AccountsTableUpdate += OnAccountTableUpdate;

            _isAccountSelected = this.WhenAnyValue(x => x.CurrentAccount).Select(x => x is not null).ToProperty(this, x => x.IsAccountSelected);
            _isAccountNotSelected = this.WhenAnyValue(x => x.CurrentAccount).Select(x => x is null).ToProperty(this, x => x.IsAccountNotSelected);

            ClosingCommand = ReactiveCommand.CreateFromTask<CancelEventArgs>(ClosingTask);

            ShowDecider = new();
            Selector = new();

            this.WhenAnyValue(x => x.IsAccountSelected).Subscribe(x =>
            {
                if (x) ShowDecider.ShowNormalTab = true;
            });
            this.WhenAnyValue(x => x.ShowDecider.ShowNoAccountTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.NoAccount);
            });
            this.WhenAnyValue(x => x.ShowDecider.ShowNormalTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.Normal);
            });
            this.WhenAnyValue(x => x.ShowDecider.ShowAddAccountTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.AddAccount);
            });
            this.WhenAnyValue(x => x.ShowDecider.ShowAddAccountsTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.AddAccounts);
            });
            this.WhenAnyValue(x => x.ShowDecider.ShowEditAccountTab).Subscribe(x =>
            {
                if (x) SetTab(TabType.EditAccount);
            });
            this.WhenAnyValue(x => x.TabSelector).Subscribe(x =>
            {
                switch (x)
                {
                    case TabType.NoAccount:
                        ShowDecider.ShowNoAccountTab = true;
                        break;

                    case TabType.Normal:
                        ShowDecider.ShowNormalTab = true;
                        break;

                    case TabType.AddAccount:
                        ShowDecider.ShowAddAccountTab = true;
                        break;

                    case TabType.AddAccounts:
                        ShowDecider.ShowAddAccountsTab = true;
                        break;

                    case TabType.EditAccount:
                        ShowDecider.ShowEditAccountTab = true;
                        break;
                }
            });
        }

        private async Task ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            _waitingWindow.ViewModel.Show("saving data");
            await Task.Run(async () =>
            {
                using var context = _contextFactory.CreateDbContext();
                var accounts = context.Accounts.ToList();

                if (accounts.Any())
                {
                    var pauseTasks = new List<Task>();
                    foreach (var account in accounts)
                    {
                        pauseTasks.Add(Pause(account.Id));
                    }
                    await Task.WhenAll(pauseTasks);
                }

                _planManager.Save();

                var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
                if (Directory.Exists(path)) Directory.Delete(path, true);

                App.Provider.Dispose();
            });

            var mainWindow = App.GetService<MainWindow>();
            mainWindow.Hide();

            _closed = true;
            _waitingWindow.ViewModel.Close();
            mainWindow.Close();
        }

        private async Task Pause(int index)
        {
            var status = _taskManager.GetAccountStatus(index);
            if (status == AccountStatus.Paused)
            {
                _taskManager.UpdateAccountStatus(index, AccountStatus.Online);
                return;
            }

            if (status == AccountStatus.Online)
            {
                var current = _taskManager.GetCurrentTask(index);
                _taskManager.UpdateAccountStatus(index, AccountStatus.Pausing);
                if (current is not null)
                {
                    current.Cts.Cancel();
                    _waitingWindow.ViewModel.Show("waiting current task stops");
                    await Task.Run(() =>
                    {
                        while (current.Stage != TaskStage.Waiting) { }
                    });
                    _waitingWindow.ViewModel.Close();
                }
                _taskManager.UpdateAccountStatus(index, AccountStatus.Paused);
                return;
            }
        }

        private void SetTab(TabType tab)
        {
            switch (tab)
            {
                case TabType.NoAccount:
                    CurrentIndex = -1;
                    ShowDecider.ShowNormalTab = false;
                    ShowDecider.ShowAddAccountTab = false;
                    ShowDecider.ShowAddAccountsTab = false;
                    ShowDecider.ShowEditAccountTab = false;
                    Selector.SelectedNoAccount = true;
                    break;

                case TabType.Normal:
                    ShowDecider.ShowNoAccountTab = false;
                    ShowDecider.ShowAddAccountTab = false;
                    ShowDecider.ShowAddAccountsTab = false;
                    ShowDecider.ShowEditAccountTab = false;
                    Selector.SelectedNormal = true;
                    break;

                case TabType.AddAccount:
                    CurrentIndex = -1;
                    ShowDecider.ShowNoAccountTab = false;
                    ShowDecider.ShowNormalTab = false;
                    ShowDecider.ShowAddAccountsTab = false;
                    ShowDecider.ShowEditAccountTab = false;
                    Selector.SelectedAddAccount = true;
                    break;

                case TabType.AddAccounts:
                    CurrentIndex = -1;
                    ShowDecider.ShowNoAccountTab = false;
                    ShowDecider.ShowNormalTab = false;
                    ShowDecider.ShowAddAccountTab = false;
                    ShowDecider.ShowEditAccountTab = false;
                    Selector.SelectedAddAccounts = true;
                    break;

                case TabType.EditAccount:
                    ShowDecider.ShowNoAccountTab = false;
                    ShowDecider.ShowNormalTab = false;
                    ShowDecider.ShowAddAccountTab = false;
                    ShowDecider.ShowAddAccountsTab = false;
                    Selector.SelectedEditAccount = true;
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
            Account oldAccount = null;
            if (CurrentIndex > -1)
            {
                oldAccount = Accounts[CurrentIndex];
            }

            Accounts.Clear();

            if (context.Accounts.Any())
            {
                foreach (var item in context.Accounts)
                {
                    Accounts.Add(item);
                }

                var account = context.Accounts.Find(oldAccount?.Id);
                if (!ShowDecider.IsShowSubTab())
                {
                    if (account is not null) CurrentIndex = Accounts.IndexOf(account);
                    else CurrentIndex = 0;
                }
            }
            else
            {
                ShowDecider.ShowNoAccountTab = true;
            }
        }

        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly EventManager _eventManager;
        private readonly ITaskManager _taskManager;

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

        private ShowDecider _showDecider;

        public ShowDecider ShowDecider
        {
            get => _showDecider;
            set => this.RaiseAndSetIfChanged(ref _showDecider, value);
        }

        private Selector _selector;

        public Selector Selector
        {
            get => _selector;
            set => this.RaiseAndSetIfChanged(ref _selector, value);
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

    public class Selector : ReactiveObject
    {
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
    }

    public class ShowDecider : ReactiveObject
    {
        public bool IsShowSubTab()
        {
            return ShowAddAccountTab || ShowAddAccountsTab || ShowEditAccountTab;
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
    }
}