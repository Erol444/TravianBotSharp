using DynamicData;
using DynamicData.Kernel;
using MainCore.Tasks.Update;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.FarmingView
{
    public class FarmListViewModel : AccountTabBaseViewModel
    {
        public FarmListViewModel()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask(RefreshTask);
            this.WhenAnyValue(vm => vm.CurrentFarm).BindTo(_selectorViewModel, vm => vm.Farm);
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

            LoadData(accountId);
        }

        private void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var farms = context.Farms
                .Where(x => x.AccountId == index)
                .AsList()
                .Select(farm =>
                {
                    var farmSetting = context.FarmsSettings.Find(farm.Id);
                    var color = farmSetting.IsActive ? Color.ForestGreen.ToMediaColor() : Color.Red.ToMediaColor();
                    return new ListBoxItem(farm.Id, farm.Name, color);
                }).ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                FarmList.Clear();
                FarmList.AddRange(farms);
                if (farms.Any())
                {
                    CurrentFarm = FarmList[0];
                }
                else
                {
                    CurrentFarm = null;
                }
            });
        }

        private Task RefreshTask()
        {
            var accountId = _selectorViewModel.Account.Id;
            var tasks = _taskManager.GetList(accountId);
            if (!tasks.Any(x => x.GetType() == typeof(UpdateFarmList)))
            {
                _taskManager.Add(accountId, new UpdateFarmList(accountId));
            }
            return Task.CompletedTask;
        }

        public ObservableCollection<ListBoxItem> FarmList { get; } = new();
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

        private ListBoxItem _currentFarm;

        public ListBoxItem CurrentFarm
        {
            get => _currentFarm;
            set => this.RaiseAndSetIfChanged(ref _currentFarm, value);
        }
    }
}