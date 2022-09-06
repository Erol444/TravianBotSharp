using MainCore;
using MainCore.Enums;
using MainCore.Services;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Sim;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Interfaces;
using WPFUI.Views;

namespace WPFUI.ViewModels.Tabs
{
    public class GeneralViewModel : ReactiveObject, IMainTabPage
    {
        public GeneralViewModel()
        {
            _eventManager = App.GetService<IEventManager>();
            _eventManager.AccountStatusUpdate += OnAccountStatusUpdate;
            _taskManager = App.GetService<ITaskManager>();
            _planManager = App.GetService<IPlanManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _waitingWindow = App.GetService<WaitingWindow>();

            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask, this.WhenAnyValue(x => x.IsValidStatus));
            RestartCommand = ReactiveCommand.Create(RestartTask, this.WhenAnyValue(x => x.IsValidRestart));

            this.WhenAnyValue(x => x.AccountId).Subscribe(LoadData);
        }

        private void OnAccountStatusUpdate()
        {
            RxApp.MainThreadScheduler.Schedule(() => LoadData(AccountId));
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        public async Task PauseTask()
        {
            await Pause(AccountId);
        }

        public void RestartTask()
        {
            Restart(AccountId);
        }

        private void LoadData(int index)
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
                    await Observable.Start(() =>
                    {
                        while (current.Stage != TaskStage.Waiting) { }
                    }, RxApp.TaskpoolScheduler);
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

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }

        private readonly Random rand = new();

        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;
        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly WaitingWindow _waitingWindow;
    }
}