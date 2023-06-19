using MainCore;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.FunctionTasks;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using Access = MainCore.Models.Database.Access;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class MainButtonPanelViewModel : AccountTabBaseViewModel
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;
        private readonly IPlanManager _planManager;
        private readonly IRestClientManager _restClientManager;
        private readonly ILogHelper _logHelper;
        private readonly ITimerManager _timeManager;
        private readonly IChromeManager _chromeManager;

        private readonly WaitingOverlayViewModel _waitingOverlay;
        private readonly VersionOverlayViewModel _versionWindow;

        private readonly MainTabPanelViewModel _mainTabPanelViewModel;
        private readonly AccountListViewModel _accountListViewModel;
        private readonly IAccessHelper _accessHelper;

        public MainButtonPanelViewModel(SelectorViewModel selectorViewModel, IDbContextFactory<AppDbContext> contextFactory, IEventManager eventManager, ITaskManager taskManager, IPlanManager planManager, IRestClientManager restClientManager, ILogHelper logHelper, ITimerManager timeManager, IChromeManager chromeManager, WaitingOverlayViewModel waitingWindow, VersionOverlayViewModel versionWindow, MainTabPanelViewModel mainTabPanelViewModel, AccountListViewModel accountListViewModel, IAccessHelper accessHelper) : base(selectorViewModel)
        {
            _contextFactory = contextFactory;
            _eventManager = eventManager;
            _taskManager = taskManager;
            _planManager = planManager;
            _restClientManager = restClientManager;
            _logHelper = logHelper;
            _timeManager = timeManager;
            _chromeManager = chromeManager;
            _waitingOverlay = waitingWindow;
            _versionWindow = versionWindow;
            _mainTabPanelViewModel = mainTabPanelViewModel;
            _accountListViewModel = accountListViewModel;
            _accessHelper = accessHelper;

            _eventManager.AccountStatusUpdate += OnAccountUpdate;

            CheckVersionCommand = ReactiveCommand.Create(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);

            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm._selectorViewModel.IsAccountSelected, (a, b) => a && b));

            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm._selectorViewModel.IsAccountSelected, (a, b) => a && b));
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask, this.WhenAnyValue(vm => vm.IsAllowLogout, vm => vm._selectorViewModel.IsAccountSelected, (a, b) => a && b));

            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask, this.WhenAnyValue(x => x.IsValidStatus));
            RestartCommand = ReactiveCommand.Create(RestartTask, this.WhenAnyValue(x => x.IsValidRestart));
        }

        protected override void Init(int accountId)
        {
            var status = _taskManager.GetAccountStatus(accountId);
            LoadData(status);
        }

        private void OnAccountUpdate(int accountId, AccountStatus status)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;
            LoadData(status);
        }

        private void LoadData(AccountStatus status)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                switch (status)
                {
                    case AccountStatus.Offline:
                        IsAllowLogin = true;
                        IsAllowLogout = false;

                        IsValidStatus = false;
                        TextPause = "Offline";
                        break;

                    case AccountStatus.Starting:
                        IsAllowLogin = false;
                        IsAllowLogout = false;

                        IsValidStatus = false;
                        TextPause = "Starting";
                        break;

                    case AccountStatus.Online:
                        IsAllowLogin = false;
                        IsAllowLogout = true;

                        IsValidStatus = true;
                        TextPause = "Pause";
                        break;

                    case AccountStatus.Pausing:
                        IsAllowLogin = false;
                        IsAllowLogout = false;

                        IsValidStatus = false;
                        TextPause = "Pausing";
                        break;

                    case AccountStatus.Paused:
                        IsAllowLogin = false;
                        IsAllowLogout = true;

                        IsValidStatus = true;
                        TextPause = "Resume";
                        break;

                    case AccountStatus.Stopping:
                        IsAllowLogin = false;
                        IsAllowLogout = false;

                        IsValidStatus = false;
                        TextPause = "Stopping";
                        break;
                }

                IsValidRestart = status == AccountStatus.Paused;
            });
        }

        private void CheckVersionTask()
        {
            _versionWindow.OpenCommand.Execute().Subscribe();
        }

        private void AddAccountTask()
        {
            _mainTabPanelViewModel.SetTab(TabType.AddAccount);
            _accountListViewModel.CurrentAccount = null;
        }

        private void AddAccountsTask()
        {
            _mainTabPanelViewModel.SetTab(TabType.AddAccounts);
            _accountListViewModel.CurrentAccount = null;
        }

        private Task LoginTask() => Task.Run(() => LoginAccount(AccountId));

        private Task LogoutTask() => Task.Run(() => LogoutAccount(AccountId));

        private async Task DeleteAccountTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);

            if (MessageBox.Show($"Do you want to delete account {account.Username} ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _waitingOverlay.ShowCommand.Execute("saving data").Subscribe();
                await Task.Run(() => DeleteAccount(AccountId));
                _eventManager.OnAccountsUpdate();
                _waitingOverlay.CloseCommand.Execute().Subscribe();
            }
        }

        public Task PauseTask() => Pause(AccountId);

        public void RestartTask() => Restart(AccountId);

        private void LoginAccount(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountInfo = context.AccountsInfo.Find(index);
            if (accountInfo.Tribe == TribeEnums.Any)
            {
                MessageBox.Show("You have to choose tribe in Settings tab first");
                return;
            }
            _taskManager.UpdateAccountStatus(index, AccountStatus.Starting);
            var accesses = context.Accesses.Where(x => x.AccountId == index).OrderBy(x => x.LastUsed);
            Access selectedAccess = null;
            foreach (var access in accesses)
            {
                if (string.IsNullOrEmpty(access.ProxyHost))
                {
                    selectedAccess = access;
                    access.LastUsed = DateTime.Now;
                    context.SaveChanges();
                    break;
                }

                _logHelper.Information(index, $"Checking proxy {access.ProxyHost}");
                var result = _accessHelper.IsValid(_restClientManager.Get(new(access)));
                if (result)
                {
                    _logHelper.Information(index, $"Proxy {access.ProxyHost} is working");
                    selectedAccess = access;
                    access.LastUsed = DateTime.Now;
                    context.SaveChanges();
                    break;
                }
                else
                {
                    _logHelper.Information(index, $"Proxy {access.ProxyHost} is not working");
                }
            }

            if (selectedAccess is null)
            {
                _taskManager.UpdateAccountStatus(index, AccountStatus.Offline);
                _logHelper.Information(index, "All proxy of this account is not working");
                MessageBox.Show("All proxy of this account is not working");
                return;
            }

            var chromeBrowser = _chromeManager.Get(index);
            var setting = context.AccountsSettings.Find(index);
            var account = context.Accounts.Find(index);
            try
            {
                chromeBrowser.Setup(selectedAccess, setting);

                chromeBrowser.Navigate(account.Server);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
            }
            _taskManager.Add<LoginTask>(index, true);

            var sleepExist = _taskManager.GetList(index).OfType<SleepTask>().FirstOrDefault();
            if (sleepExist is null)
            {
                (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);
                var time = TimeSpan.FromMinutes(Random.Shared.Next(min, max));
                _taskManager.Add<SleepTask>(index, () => new(index) { ExecuteAt = DateTime.Now.Add(time) });
            }

            _timeManager.Start(index);
            _taskManager.UpdateAccountStatus(index, AccountStatus.Online);
        }

        private void LogoutAccount(int index)
        {
            _taskManager.UpdateAccountStatus(index, AccountStatus.Stopping);

            var current = _taskManager.GetCurrentTask(index);
            if (current is not null)
            {
                _taskManager.StopCurrentTask(index);
                while (current.Stage != TaskStage.Waiting) { }
            }

            _chromeManager.Get(index).Close();
            _taskManager.UpdateAccountStatus(index, AccountStatus.Offline);
        }

        private void DeleteAccount(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            context.DeleteAccount(index);
            context.SaveChanges();
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
                    await Task.Run(() =>
                    {
                        while (current.Stage != TaskStage.Waiting) { }
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

        public ReactiveCommand<Unit, Unit> CheckVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> PauseCommand { get; }
        public ReactiveCommand<Unit, Unit> RestartCommand { get; }

        private bool _isAllowLogout;

        public bool IsAllowLogout
        {
            get => _isAllowLogout;
            set => this.RaiseAndSetIfChanged(ref _isAllowLogout, value);
        }

        private bool _isAllowLogin;

        public bool IsAllowLogin
        {
            get => _isAllowLogin;
            set => this.RaiseAndSetIfChanged(ref _isAllowLogin, value);
        }

        private bool _isValidStatus;

        public bool IsValidStatus
        {
            get => _isValidStatus;
            set => this.RaiseAndSetIfChanged(ref _isValidStatus, value);
        }

        private bool _isValidRestart;

        public bool IsValidRestart
        {
            get => _isValidRestart;
            set => this.RaiseAndSetIfChanged(ref _isValidRestart, value);
        }

        private string _textPause;

        public string TextPause
        {
            get => _textPause;
            set => this.RaiseAndSetIfChanged(ref _textPause, value);
        }
    }
}