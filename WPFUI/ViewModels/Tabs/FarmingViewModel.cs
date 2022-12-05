using MainCore.Tasks.Attack;
using MainCore.Tasks.Update;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class FarmingViewModel : AccountTabViewModelBase
    {
        public FarmingViewModel()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask(RefreshTask);
            StartCommand = ReactiveCommand.CreateFromTask(StartTask);
            StopCommand = ReactiveCommand.CreateFromTask(StopTask);

            _eventManager.FarmListUpdate += OnFarmListUpdate;
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void OnFarmListUpdate(int accountId)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;

            RxApp.MainThreadScheduler.Schedule(() => LoadData(accountId));
        }

        private void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var farms = context.Farms.Where(x => x.AccountId == index);
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                FarmList.Clear();
                foreach (var farm in farms)
                {
                    var farmSetting = context.FarmsSettings.Find(farm.Id);
                    var color = farmSetting.IsActive ? "Green" : "Red";
                    var f = new FarmInfo() { Id = farm.Id, Name = farm.Name, Color = color };
                    FarmList.Add(f);
                }
            });
        }

        private async Task RefreshTask()
        {
            await Task.Run(() =>
            {
                var accountId = _selectorViewModel.Account.Id;
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
                var accountId = AccountId;
                using var context = _contextFactory.CreateDbContext();
                var farms = context.Farms.Where(x => x.AccountId == accountId);
                foreach (var farm in farms)
                {
                    var farmSetting = context.FarmsSettings.Find(farm.Id);
                    if (farmSetting.IsActive)
                    {
                        var tasks = _taskManager.GetList(accountId);
                        var task = tasks.Where(x => x is StartFarmList).OfType<StartFarmList>().FirstOrDefault(x => x.FarmId == farm.Id);
                        if (task is null)
                        {
                            _taskManager.Add(accountId, new StartFarmList(accountId, farm.Id));
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
                var accountId = AccountId;
                var tasks = _taskManager.GetList(accountId);
                var farmLists = tasks.Where(x => x.GetType() == typeof(StartFarmList));
                foreach (var farm in farmLists)
                {
                    _taskManager.Remove(accountId, farm);
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
    }
}