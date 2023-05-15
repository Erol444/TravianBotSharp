using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Tasks.FunctionTasks;
using ReactiveUI;
using Splat;
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
        private readonly MainTabPanelViewModel _mainTabPanelViewModel;
        private readonly IAccessHelper _accessHelper;

        public MainButtonPanelViewModel()
        {
            _eventManager.AccountStatusUpdate += OnAccountUpdate;
            _mainTabPanelViewModel = Locator.Current.GetService<MainTabPanelViewModel>();
            _accessHelper = Locator.Current.GetService<IAccessHelper>();

            CheckVersionCommand = ReactiveCommand.Create(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.Create(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.Create(AddAccountsTask);

            DeleteAccountCommand = ReactiveCommand.Create(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm._selectorViewModel.IsAccountSelected, (a, b) => a && b));

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
            _versionWindow.Show();
        }

        private void AddAccountTask()
        {
            _mainTabPanelViewModel.SetTab(TabType.AddAccount);
        }

        private void AddAccountsTask()
        {
            _mainTabPanelViewModel.SetTab(TabType.AddAccounts);
        }

        private Task LoginTask() => Task.Run(() => LoginAccount(AccountId));

        private Task LogoutTask() => Task.Run(() => LogoutAccount(AccountId));

        private void DeleteAccountTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);

            if (MessageBox.Show($"Do you want to delete account {account.Username} ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _waitingWindow.Show("saving data");
                DeleteAccount(AccountId);
                _eventManager.OnAccountsUpdate();
                _waitingWindow.Close();
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
            _logManager.AddAccount(index);
            var accesses = context.Accesses.Where(x => x.AccountId == index).OrderBy(x => x.LastUsed);
            Access selectedAccess = null;
            foreach (var access in accesses)
            {
                if (string.IsNullOrEmpty(access.ProxyHost))
                {
                    selectedAccess = access;
                    break;
                }

                _logManager.Information(index, $"Checking proxy {access.ProxyHost}");
                var result = _accessHelper.IsValid(_restClientManager.Get(new(access)));
                if (result)
                {
                    _logManager.Information(index, $"Proxy {access.ProxyHost} is working");
                    selectedAccess = access;
                    access.LastUsed = DateTime.Now;
                    context.SaveChanges();
                    break;
                }
                else
                {
                    _logManager.Information(index, $"Proxy {access.ProxyHost} is not working");
                }
            }

            if (selectedAccess is null)
            {
                _taskManager.UpdateAccountStatus(index, AccountStatus.Offline);
                _logManager.Information(index, "All proxy of this account is not working");
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
            _taskManager.Add(index, new LoginTask(index), true);

            var sleepExist = _taskManager.GetList(index).FirstOrDefault(x => x.GetType() == typeof(SleepTask));
            if (sleepExist is null)
            {
                (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);
                var time = TimeSpan.FromMinutes(Random.Shared.Next(min, max));
                _taskManager.Add(index, new SleepTask(index) { ExecuteAt = DateTime.Now.Add(time) });
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
                    _taskManager.Add(index, new UpgradeBuilding(village.Id, index));
                }
                var villageSetting = context.VillagesSettings.Find(village.Id);
                if (villageSetting.IsAutoRefresh)
                {
                    _taskManager.Add(index, new RefreshVillage(village.Id, index));
                }

                if (villageSetting.BarrackTroop != 0 || villageSetting.StableTroop != 0 || villageSetting.WorkshopTroop != 0)
                {
                    _taskManager.Add(AccountId, new TrainTroopsTask(village.Id, AccountId));
                }
            }

            var setting = context.AccountsSettings.Find(index);
            (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);

            var time = TimeSpan.FromMinutes(Random.Shared.Next(min, max));
            _taskManager.Add(index, new SleepTask(index) { ExecuteAt = DateTime.Now.Add(time) });
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