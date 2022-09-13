using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
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
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        protected override void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var farms = context.Farms.Where(x => x.AccountId == index);
            FarmList.Clear();
            ActiveFarmList.Clear();
            foreach (var farm in farms)
            {
                var f = new FarmInfo() { Id = farm.Id, Name = farm.Name, };
                FarmList.Add(f);
                var farmSetting = context.FarmsSettings.Find(farm.Id);
                if (farmSetting.IsActive)
                {
                    ActiveFarmList.Add(f);
                }
            }
        }

        private async Task RefreshTask()
        {
            await Task.Run(() => { });
        }

        private async Task StartTask()
        {
            await Task.Run(() => { });
        }

        private async Task StopTask()
        {
            await Task.Run(() => { });
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
        public ObservableCollection<FarmInfo> FarmList { get; } = new();
        public ObservableCollection<FarmInfo> ActiveFarmList { get; } = new();

        private FarmInfo _currentFarm;

        public FarmInfo CurrentFarm
        {
            get => _currentFarm;
            set => this.RaiseAndSetIfChanged(ref _currentFarm, value);
        }
    }
}