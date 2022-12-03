using DynamicData;
using MainCore;
using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Tabs;
using WPFUI.Views;
using WPFUI.Views.Tabs;

namespace WPFUI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, ITabPage
    {
        public MainWindowViewModel()
        {
            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
            _planManager = Locator.Current.GetService<IPlanManager>();
            _taskManager = Locator.Current.GetService<ITaskManager>();

            _waitingWindow = Locator.Current.GetService<WaitingViewModel>();

            _eventManager = Locator.Current.GetService<IEventManager>();
            _eventManager.AccountsTableUpdate += OnAccountTableUpdate;

            _isAccountSelected = this.WhenAnyValue(x => x.CurrentAccount).Select(x => x is not null).ToProperty(this, x => x.IsAccountSelected);
            _isAccountNotSelected = this.WhenAnyValue(x => x.CurrentAccount).Select(x => x is null).ToProperty(this, x => x.IsAccountNotSelected);
            this.WhenAnyValue(x => x.CurrentIndex).Subscribe(x =>
            {
                if (x == -1) return;
                if (_current == TabType.EditAccount) return;
                SetTab(TabType.Normal);
            });

            _tabsHolder = new()
            {
                {
                    TabType.NoAccount, new TabItemViewModel[]
                    {
                        new("No account", new NoAccountPage()) ,
                    }
                },
                {
                    TabType.AddAccount, new TabItemViewModel[]
                    {
                        new("Add account", new AddAccountPage()),
                    }
                },
                {
                    TabType.AddAccounts, new TabItemViewModel[]
                    {
                        new("Add accounts", new AddAccountsPage()),
                    }
                },
                {
                    TabType.EditAccount, new TabItemViewModel[]
                    {
                        new("Edit account", new EditAccountPage()),
                    }
                },
                {
                    TabType.Normal, new TabItemViewModel[]
                    {
                        new("General", new GeneralPage()),
                        new("Settings", new SettingsPage()),
                        new("Hero", new HeroPage()),
                        new("Villages", new VillagesPage()),
                        new("Farming", new FarmingPage()),
                        new("Debug", new DebugPage()),
                    }
                }
            };

            Tabs = new()
            {
                _tabsHolder[TabType.NoAccount][0],
            };

            ClosingCommand = ReactiveCommand.CreateFromTask<CancelEventArgs>(ClosingTask);
        }

        private async Task ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            _waitingWindow.Show("saving data");
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
            });

            var mainWindow = Locator.Current.GetService<MainWindow>();
            mainWindow.Hide();

            _closed = true;
            _waitingWindow.Close();
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
                    _waitingWindow.Show("waiting current task stops");
                    await Task.Run(() =>
                    {
                        while (current.Stage != TaskStage.Waiting) { }
                    });
                    _waitingWindow.Close();
                }
                _taskManager.UpdateAccountStatus(index, AccountStatus.Paused);
                return;
            }
        }

        private void SetTab(TabType tab)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Tabs.Clear();
                Tabs.AddRange(_tabsHolder[tab]);
                _current = tab;
            });
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
                if (_current == TabType.AddAccount || _current == TabType.AddAccounts || _current == TabType.EditAccount)
                {
                    if (account is not null) CurrentIndex = Accounts.IndexOf(account);
                    else CurrentIndex = 0;
                }
            }
            else
            {
                SetTab(TabType.NoAccount);
            }
        }

        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;

        private readonly WaitingViewModel _waitingWindow;

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

        public ReactiveCommand<CancelEventArgs, Unit> ClosingCommand { get; }
        public bool IsActive { get; set; }

        private readonly Dictionary<TabType, TabItemViewModel[]> _tabsHolder;
        private TabType _current;

        public ObservableCollection<TabItemViewModel> Tabs { get; }

        public Action Show;
    }
}