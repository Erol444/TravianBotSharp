using MainCore.Tasks.Attack;
using MainCore.Tasks.Update;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class FarmingViewModel : AccountTabBaseViewModel, IMainTabPage
    {
        public FarmingViewModel() : base()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask(RefreshTask);
            StartCommand = ReactiveCommand.CreateFromTask(StartTask);
            StopCommand = ReactiveCommand.CreateFromTask(StopTask);

            _eventManager.FarmListUpdated += OnFarmListUpdate;
            this.WhenAnyValue(x => x.IsActiveChange).Subscribe(ChangeColor);
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        private void OnFarmListUpdate(int index) => RxApp.MainThreadScheduler.Schedule(() => LoadData(index));

        public void ChangeColor(bool value)
        {
            if (CurrentFarm is not null) CurrentFarm.Color = value ? "Green" : "Red";
        }

        protected override void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var farms = context.Farms.Where(x => x.AccountId == index);
            FarmList.Clear();
            foreach (var farm in farms)
            {
                var farmSetting = context.FarmsSettings.Find(farm.Id);
                var color = farmSetting.IsActive ? "Green" : "Red";
                var f = new FarmInfo() { Id = farm.Id, Name = farm.Name, Color = color };
                FarmList.Add(f);
            }
        }

        private async Task RefreshTask()
        {
            await Task.Run(() =>
            {
                var accountId = AccountId;
                var tasks = _taskManager.GetList(accountId);
                if (!tasks.Any(x => x.GetType() == typeof(UpdateFarmList)))
                {
                    _taskManager.Add(accountId, new UpdateFarmList(accountId));
                }
            });
        }

        private async Task StartTask()
        {
            await Task.Run(() =>
            {
                using var context = _contextFactory.CreateDbContext();
                var farms = context.Farms.Where(x => x.AccountId == AccountId);
                foreach (var farm in farms)
                {
                    var farmSetting = context.FarmsSettings.Find(farm.Id);
                    if (farmSetting.IsActive)
                    {
                        var tasks = _taskManager.GetList(AccountId);
                        if (!tasks.Any(x => x.GetType() == typeof(StartFarmList) && (x as StartFarmList).FarmId == farm.Id))
                        {
                            _taskManager.Add(AccountId, new StartFarmList(AccountId, farm.Id));
                        }
                    }
                }
            });
            MessageBox.Show("Started all active farm");
        }

        private async Task StopTask()
        {
            await Task.Run(() =>
            {
                var tasks = _taskManager.GetList(AccountId);
                var farmLists = tasks.Where(x => x.GetType() == typeof(StartFarmList));
                foreach (var farm in farmLists)
                {
                    _taskManager.Remove(AccountId, farm);
                }
            });

            MessageBox.Show("Removed all farm task from queue");
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
        public ObservableCollection<FarmInfo> FarmList { get; } = new();

        private FarmInfo _currentFarm;

        public FarmInfo CurrentFarm
        {
            get => _currentFarm;
            set => this.RaiseAndSetIfChanged(ref _currentFarm, value);
        }

        private bool _isActiveChange;

        public bool IsActiveChange
        {
            get => _isActiveChange;
            set => this.RaiseAndSetIfChanged(ref _isActiveChange, value);
        }
    }
}