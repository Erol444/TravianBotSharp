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
using System.Threading.Tasks;
using WPFUI.Interfaces;

namespace WPFUI.ViewModels.Tabs
{
    public class GeneralViewModel : ReactiveObject, IMainTabPage
    {
        public int AccountId { get; set; }
        private readonly Random rand = new();

        public GeneralViewModel()
        {
            _eventManager = App.GetService<IEventManager>();
            _eventManager.AccountStatusUpdate += OnAccountStatusUpdate;
            _taskManager = App.GetService<ITaskManager>();
            _planManager = App.GetService<IPlanManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            PauseCommand = ReactiveCommand.CreateFromTask(PauseTask, this.WhenAnyValue(x => x.IsValidStatus));
            RestartCommand = ReactiveCommand.Create(RestartTask, this.WhenAnyValue(x => x.IsValidRestart));
        }

        private void OnAccountStatusUpdate()
        {
            App.Current.Dispatcher.Invoke(LoadData);
        }

        public void OnActived()
        {
            LoadData();
        }

        public async Task PauseTask()
        {
            var status = _taskManager.GetAccountStatus(AccountId);
            if (status == AccountStatus.Paused)
            {
                _taskManager.UpdateAccountStatus(AccountId, AccountStatus.Online);
                return;
            }

            if (status == AccountStatus.Online)
            {
                var current = _taskManager.GetCurrentTask(AccountId);
                _taskManager.UpdateAccountStatus(AccountId, AccountStatus.Pausing);
                if (current is not null)
                {
                    current.Cts.Cancel();
                    await Task.Run(() =>
                    {
                        while (current.Stage != TaskStage.Waiting) { }
                    });
                }
                _taskManager.UpdateAccountStatus(AccountId, AccountStatus.Paused);
                return;
            }
        }

        public void RestartTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            _taskManager.Clear(AccountId);

            foreach (var village in villages)
            {
                var queue = _planManager.GetList(village.Id);
                if (queue.Any())
                {
                    _taskManager.Add(AccountId, new UpgradeBuilding(village.Id, AccountId));
                }
            }
            var setting = context.AccountsSettings.Find(AccountId);
            (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);

            var time = TimeSpan.FromMinutes(rand.Next(min, max));
            _taskManager.Add(AccountId, new SleepTask(AccountId) { ExecuteAt = DateTime.Now.Add(time) });
            _taskManager.UpdateAccountStatus(AccountId, AccountStatus.Online);
        }

        private void LoadData()
        {
            var status = _taskManager.GetAccountStatus(AccountId);
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

        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;
        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}