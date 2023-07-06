using DynamicData;
using DynamicData.Kernel;
using MainCore;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.FunctionTasks;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.ViewModels.Uc
{
    public class MainLayoutViewModel : ViewModelBase
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;
        private readonly IAccessHelper _accessHelper;
        private readonly IChromeManager _chromeManager;
        private readonly ITimerManager _timeManager;
        private readonly IPlanManager _planManager;

        private readonly SelectedItemStore _selectedItemStore;

        private readonly VersionOverlayViewModel _versionWindow;
        private readonly WaitingOverlayViewModel _waitingOverlay;

        private readonly NoAccountViewModel _noAccountViewModel;
        private readonly AddAccountViewModel _addAccountViewModel;
        private readonly AddAccountsViewModel _addAccountsViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly HeroViewModel _heroViewModel;
        private readonly VillagesViewModel _villagesViewModel;
        private readonly FarmingViewModel _farmingViewModel;
        private readonly EditAccountViewModel _editAccountViewModel;
        private readonly DebugViewModel _debugViewModel;

        public AccountTabStore AccountTabStore { get; } = new();

        public MainLayoutViewModel(IDbContextFactory<AppDbContext> contextFactory, IEventManager eventManager, SelectedItemStore selectedItemStore, VersionOverlayViewModel versionWindow, WaitingOverlayViewModel waitingOverlay, ITaskManager taskManager, IChromeManager chromeManager, IPlanManager planManager, NoAccountViewModel noAccountViewModel, AddAccountViewModel addAccountViewModel, AddAccountsViewModel addAccountsViewModel, SettingsViewModel settingsViewModel, HeroViewModel heroViewModel, VillagesViewModel villagesViewModel, FarmingViewModel farmingViewModel, EditAccountViewModel editAccountViewModel, DebugViewModel debugViewModel, ITimerManager timeManager, IAccessHelper accessHelper)
        {
            _contextFactory = contextFactory;
            _eventManager = eventManager;
            _taskManager = taskManager;
            _chromeManager = chromeManager;
            _planManager = planManager;
            _timeManager = timeManager;
            _accessHelper = accessHelper;

            _selectedItemStore = selectedItemStore;

            _versionWindow = versionWindow;
            _waitingOverlay = waitingOverlay;

            _noAccountViewModel = noAccountViewModel;
            _addAccountViewModel = addAccountViewModel;
            _addAccountsViewModel = addAccountsViewModel;
            _settingsViewModel = settingsViewModel;
            _heroViewModel = heroViewModel;
            _villagesViewModel = villagesViewModel;
            _farmingViewModel = farmingViewModel;
            _editAccountViewModel = editAccountViewModel;
            _debugViewModel = debugViewModel;

            _eventManager.AccountsTableUpdate += OnAccountsTableUpdate;
            _eventManager.AccountStatusUpdate += OnAccountStatusUpdate;

            _selectedItemStore.AccountChanged += OnAccountChanged;

            CheckVersionCommand = ReactiveCommand.Create(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);

            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask, this.WhenAnyValue(vm => vm.IsAllowLogin));
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask, this.WhenAnyValue(vm => vm.IsAllowLogout));
            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask, this.WhenAnyValue(x => x.IsAllowPause));
            RestartCommand = ReactiveCommand.Create(RestartTask, this.WhenAnyValue(x => x.IsAllowRestart));

            var currentAccountObservable = this.WhenAnyValue(x => x.CurrentAccount);
            currentAccountObservable.BindTo(_selectedItemStore, vm => vm.Account);
            currentAccountObservable.Subscribe(x =>
            {
                var tabType = TabType.Normal;
                if (x is null) tabType = TabType.NoAccount;
                AccountTabStore.SetTabType(tabType);
            });
        }

        #region Event

        private void OnAccountStatusUpdate(int accountId, AccountStatus status)
        {
            var account = Accounts.FirstOrDefault(x => x.Id == accountId);
            if (account is null) return;

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Status = status;
                account.Color = status.GetColor().ToMediaColor();
            });
        }

        private void OnAccountsTableUpdate()
        {
            LoadAccountList();
        }

        private void OnAccountChanged(int accountId)
        {
            if (CurrentAccount is null) return;
            if (CurrentAccount.Id != accountId) return;
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Status = _taskManager.GetAccountStatus(accountId);
            });
        }

        #endregion Event

        #region Load

        public void LoadData()
        {
            LoadAccountList();
        }

        private void LoadAccountList()
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
                }
                else
                {
                    CurrentAccount = null;
                }
            });
        }

        #endregion Load

        #region Task

        private void CheckVersionTask()
        {
            _versionWindow.OpenCommand.Execute().Subscribe();
        }

        private void AddAccountTask()
        {
            CurrentAccount = null;
            AccountTabStore.SetTabType(TabType.AddAccount);
        }

        private void AddAccountsTask()
        {
            CurrentAccount = null;
            AccountTabStore.SetTabType(TabType.AddAccounts);
        }

        private async Task DeleteAccountTask()
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("No account selected");
                return;
            }

            await Task.Run(() => DeleteAccount(CurrentAccount.Id));
        }

        private async Task LoginTask()
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("No account selected");
                return;
            }

            await Task.Run(() => LoginAccount(CurrentAccount.Id));
        }

        private async Task LogoutTask()
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("No account selected");
                return;
            }

            await LogoutAccount(CurrentAccount.Id);
        }

        public async Task PauseTask()
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("No account selected");
                return;
            }

            await Pause(CurrentAccount.Id);
        }

        public void RestartTask()
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("No account selected");
                return;
            }

            Restart(CurrentAccount.Id);
        }

        #endregion Task

        #region Method

        private void DeleteAccount(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(accountId);

            string messageBoxText = $"Do you want to delete account {account.Username} ?";
            var result = MessageBox.Show(messageBoxText, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _waitingOverlay.ShowCommand.Execute("saving data").Subscribe();
                context.DeleteAccount(accountId);
                context.SaveChanges();
                _eventManager.OnAccountsUpdate();
                _waitingOverlay.CloseCommand.Execute().Subscribe();
            }
        }

        private void LoginAccount(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountInfo = context.AccountsInfo.Find(accountId);
            if (accountInfo.Tribe == TribeEnums.Any)
            {
                MessageBox.Show("Choose tribe in Settings tab first");
                return;
            }

            _taskManager.UpdateAccountStatus(accountId, AccountStatus.Starting);

            var (selectedAccess, _) = _accessHelper.GetNextAccess(accountId);

            if (selectedAccess is null)
            {
                _taskManager.UpdateAccountStatus(accountId, AccountStatus.Offline);
                MessageBox.Show("All proxy of this account is not working");
                return;
            }

            var chromeBrowser = _chromeManager.Get(accountId);
            var setting = context.AccountsSettings.Find(accountId);
            var account = context.Accounts.Find(accountId);
            try
            {
                chromeBrowser.Setup(selectedAccess, setting);
                chromeBrowser.Navigate(account.Server);
            }
            catch (Exception ex)
            {
                _taskManager.UpdateAccountStatus(accountId, AccountStatus.Offline);
                MessageBox.Show(ex.Message, "Error");
                return;
            }

            _taskManager.Add<LoginTask>(accountId, true);

            var sleepTasks = _taskManager.GetList(accountId).OfType<SleepTask>();
            if (!sleepTasks.Any())
            {
                (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);
                var time = TimeSpan.FromMinutes(Random.Shared.Next(min, max));
                _taskManager.Add<SleepTask>(accountId, () => new(accountId) { ExecuteAt = DateTime.Now.Add(time) });
            }

            _timeManager.Start(accountId);
            _taskManager.UpdateAccountStatus(accountId, AccountStatus.Online);
        }

        private async Task LogoutAccount(int index)
        {
            _taskManager.UpdateAccountStatus(index, AccountStatus.Stopping);

            var current = _taskManager.GetCurrentTask(index);
            if (current is not null)
            {
                _taskManager.StopCurrentTask(index);
                await Task.Run(async () =>
                {
                    while (current.Stage != TaskStage.Waiting)
                    {
                        current = _taskManager.GetCurrentTask(index);
                        if (current is null) return;
                        await Task.Delay(500);
                    }
                });
            }

            _chromeManager.Get(index).Close();

            _taskManager.UpdateAccountStatus(index, AccountStatus.Offline);
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
                    _taskManager.StopCurrentTask(index);
                    _waitingOverlay.ShowCommand.Execute("waiting current task stops").Subscribe();
                    await Task.Run(async () =>
                    {
                        while (current.Stage != TaskStage.Waiting)
                        {
                            current = _taskManager.GetCurrentTask(index);
                            if (current is null) return;
                            await Task.Delay(500);
                        }
                    });
                    _waitingOverlay.CloseCommand.Execute().Subscribe();
                }
                _taskManager.UpdateAccountStatus(index, AccountStatus.Paused);
                return;
            }
        }

        private void Restart(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == index);
            _taskManager.Clear(index);

            foreach (var village in villages)
            {
                var queue = _planManager.GetList(village.Id);
                if (queue.Any())
                {
                    _taskManager.Add<UpgradeBuilding>(index, village.Id);
                }
                var villageSetting = context.VillagesSettings.Find(village.Id);
                if (villageSetting.IsAutoRefresh)
                {
                    _taskManager.Add<RefreshVillage>(index, village.Id);
                }

                if (villageSetting.BarrackTroop != 0 || villageSetting.StableTroop != 0 || villageSetting.WorkshopTroop != 0)
                {
                    _taskManager.Add<TrainTroopsTask>(village.Id, index);
                }
            }

            var setting = context.AccountsSettings.Find(index);
            (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);

            var time = TimeSpan.FromMinutes(Random.Shared.Next(min, max));
            _taskManager.Add<SleepTask>(index, () => new(index) { ExecuteAt = DateTime.Now.Add(time) });
            _taskManager.UpdateAccountStatus(index, AccountStatus.Online);
        }

        #endregion Method

        #region Variable

        public ObservableCollection<ListBoxItem> Accounts { get; } = new();

        private ListBoxItem _currentAccount;

        public ListBoxItem CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }

        public ReactiveCommand<Unit, Unit> CheckVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> PauseCommand { get; }
        public ReactiveCommand<Unit, Unit> RestartCommand { get; }

        private AccountStatus _status;

        public AccountStatus Status
        {
            get => _status;
            set
            {
                this.RaiseAndSetIfChanged(ref _status, value);
                IsAllowLogin = value.IsAllowLogin();
                IsAllowLogout = value.IsAllowLogout();
                IsAllowRestart = value.IsAllowRestart();
                IsAllowPause = value.IsAllowPause();
                TextPause = value.GetPauseText();
            }
        }

        private bool _isAllowLogin;

        public bool IsAllowLogin
        {
            get => _isAllowLogin;
            set => this.RaiseAndSetIfChanged(ref _isAllowLogin, value);
        }

        private bool _isAllowLogout;

        public bool IsAllowLogout
        {
            get => _isAllowLogout;
            set => this.RaiseAndSetIfChanged(ref _isAllowLogout, value);
        }

        private bool _isAllowRestart;

        public bool IsAllowRestart
        {
            get => _isAllowRestart;
            set => this.RaiseAndSetIfChanged(ref _isAllowRestart, value);
        }

        private bool _isAllowPause;

        public bool IsAllowPause
        {
            get => _isAllowPause;
            set => this.RaiseAndSetIfChanged(ref _isAllowPause, value);
        }

        private string _textPause;

        public string TextPause
        {
            get => _textPause;
            set => this.RaiseAndSetIfChanged(ref _textPause, value);
        }

        public NoAccountViewModel NoAccountViewModel => _noAccountViewModel;
        public AddAccountViewModel AddAccountViewModel => _addAccountViewModel;
        public AddAccountsViewModel AddAccountsViewModel => _addAccountsViewModel;
        public SettingsViewModel SettingsViewModel => _settingsViewModel;
        public HeroViewModel HeroViewModel => _heroViewModel;
        public VillagesViewModel VillagesViewModel => _villagesViewModel;
        public FarmingViewModel FarmingViewModel => _farmingViewModel;
        public EditAccountViewModel EditAccountViewModel => _editAccountViewModel;
        public DebugViewModel DebugViewModel => _debugViewModel;

        #endregion Variable
    }
}