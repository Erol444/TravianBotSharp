using MainCore.Tasks.Attack;
using ReactiveUI;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class FarmingViewModel : AccountTabBaseViewModel
    {
        public FarmingViewModel()
        {
            StartCommand = ReactiveCommand.CreateFromTask(StartTask);
            StopCommand = ReactiveCommand.CreateFromTask(StopTask);
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

        protected override void Init(int id)
        {
        }

        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
    }
}