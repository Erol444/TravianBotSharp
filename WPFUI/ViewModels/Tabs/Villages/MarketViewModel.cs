using MainCore.Tasks.Update;
using MainCore.Tasks.Misc;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class MarketViewModel : VillageTabBaseViewModel, ITabPage
    {
        public MarketViewModel() : base()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
        }

        protected override void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesMarket.Find(index);
            Settings.CopyFrom(settings);
        }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentVillage is not null)
            {
                LoadData(CurrentVillage.Id);
            }
        }

        public void OnDeactived()
        {
            IsActive = false;
        }


        private async Task SaveTask()
        {
            if (!Settings.IsValidate()) return;
            _waitingWindow.ViewModel.Show("saving village's settings");

            await Task.Run(() =>
            {
                var villageId = CurrentVillage.Id;
                Save(villageId);
                var accountId = CurrentAccount.Id;
                TaskBasedSetting(villageId, accountId);
            });
            _waitingWindow.ViewModel.Close();

            MessageBox.Show("Saved.");

            // Update dorf so it will start sending resources immidiately
            UpdateDorf1();
        }
        private void Save(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesMarket.Find(index);
            Settings.CopyTo(setting);
            Settings.CopyFrom(setting);
            context.Update(setting);
            context.SaveChanges();
        }

        private void UpdateDorf1()
        {
            var accountId = CurrentAccount.Id;
            var tasks = _taskManager.GetList(accountId);
            var villageId = CurrentVillage.Id;
            var updateTask = tasks.OfType<UpdateDorf1>().FirstOrDefault(x => x.VillageId == villageId);
            if (updateTask is null)
            {
                _taskManager.Add(accountId, new UpdateDorf1(villageId, accountId));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        private void TaskBasedSetting(int villageId, int accountId)
        {
            var list = _taskManager.GetList(accountId);
            {
                var tasks = list.OfType<RefreshVillage>(); // TODO: Rename RefreshVillage to SendReousrcesSomething
                if (Settings.IsSendExcessResources)
                {
                    if (!tasks.Any(x => x.VillageId == villageId))
                    {
                        // _taskManager.Add(accountId, new RefreshVillage(villageId, accountId));
                        // TODO: Execute send resources out task
                    }
                }
                else
                {
                    var updateTasks = tasks.Where(x => x.VillageId == villageId);
                    foreach (var item in updateTasks)
                    {
                        _taskManager.Remove(accountId, item);
                    }
                }
            }
        }

        public VillageMarket Settings { get; } = new();


        public bool IsActive { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    }
}