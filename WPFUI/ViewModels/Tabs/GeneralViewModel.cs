using MainCore.Enums;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Sim;
using MainCore.Tasks.Update;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Interfaces;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class GeneralViewModel : AccountTabBaseViewModel, ITabPage
    {
        public GeneralViewModel() : base()
        {
            _eventManager.AccountStatusUpdate += OnAccountStatusUpdate;

            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask, this.WhenAnyValue(x => x.IsValidStatus));
            RestartCommand = ReactiveCommand.Create(RestartTask, this.WhenAnyValue(x => x.IsValidRestart));
        }

        private void OnAccountStatusUpdate(int accountId)
        {
            if (!IsActive) return;
            if (CurrentAccount is null) return;
            if (CurrentAccount.Id != accountId) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadData(accountId));
        }

        public bool IsActive { get; set; }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentAccount is not null)
            {
                LoadData(CurrentAccount.Id);
            }
        }

        public void OnDeactived()
        {
            IsActive = false;
        }

        public Task PauseTask() => Pause(CurrentAccount.Id);

        public void RestartTask() => Restart(CurrentAccount.Id);

        protected override void LoadData(int index)
        {
            var status = _taskManager.GetAccountStatus(index);
            switch (status)
            {
                case AccountStatus.Offline:
                case AccountStatus.Starting:
                case AccountStatus.Pausing:
                case AccountStatus.Stopping:
                    IsValidStatus = false;
                    PauseText = "~";
                    break;

                case AccountStatus.Online:
                    IsValidStatus = true;
                    PauseText = "Pause";
                    break;

                case AccountStatus.Paused:
                    IsValidStatus = true;
                    PauseText = "Resume";
                    break;

                default:
                    break;
            }

            IsValidRestart = status == AccountStatus.Paused;

            Status = status.ToString();
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
                    _taskManager.Add(index, new UpdateDorf1(village.Id, index));
                }
            }

            var setting = context.AccountsSettings.Find(index);
            (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);

            var time = TimeSpan.FromMinutes(rand.Next(min, max));
            _taskManager.Add(index, new SleepTask(index) { ExecuteAt = DateTime.Now.Add(time) });
            _taskManager.UpdateAccountStatus(index, AccountStatus.Online);
        }

        public ReactiveCommand<Unit, Unit> PauseCommand { get; }
        public ReactiveCommand<Unit, Unit> RestartCommand { get; }

        private string _status;

        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        private string _pauseText;

        public string PauseText
        {
            get => _pauseText;
            set => this.RaiseAndSetIfChanged(ref _pauseText, value);
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

        private readonly Random rand = new();
    }
}