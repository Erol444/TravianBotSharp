using MainCore.Tasks.Attack;
using ReactiveUI;
using System;
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
                var farms = context.Farms.Where(x => x.AccountId == accountId).ToList();
                var activeFarms = farms.Where(x =>
                {
                    var farmSetting = context.FarmsSettings.Find(x.Id);
                    return farmSetting.IsActive;
                }).ToList();
                if (activeFarms.Count == 0) return;
                var tasks = _taskManager.GetList(AccountId);
                var task = tasks.OfType<StartFarmList>().FirstOrDefault();
                if (task is null)
                {
                    _taskManager.Add(accountId, new StartFarmList(accountId));
                }
                else
                {
                    task.ExecuteAt = DateTime.Now;
                    _taskManager.Update(AccountId);
                }
            });

            MessageBox.Show("Send all active farm");
        }

        private async Task StopTask()
        {
            await Task.Run(() =>
            {
                var accountId = AccountId;
                var tasks = _taskManager.GetList(accountId);
                var task = tasks.OfType<StartFarmList>().FirstOrDefault();
                if (task is null) return;

                _taskManager.Remove(accountId, task);
            });

            MessageBox.Show("Removed send farm task from queue");
        }

        protected override void Init(int id)
        {
        }

        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
    }
}