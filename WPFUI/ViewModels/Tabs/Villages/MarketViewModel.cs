using MainCore.Enums;
using MainCore.Helper;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Sim;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
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
            // using var context = _contextFactory.CreateDbContext();
            // var updateTime = context.VillagesUpdateTime.Find(index);
            // var dorf1 = updateTime.Dorf1;
            // var dorf2 = updateTime.Dorf2;
            // LastUpdate = dorf1 > dorf2 ? dorf1 : dorf2;
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


        private Resources _ratio = new();

        public Resources SendOutLimit
        {
            get => _ratio;
            set => this.RaiseAndSetIfChanged(ref _ratio, value);
        }

        private bool _isSendExcessResources = new();

        public bool IsSendExcessResources
        {
            get => _isSendExcessResources;
            set => this.RaiseAndSetIfChanged(ref _isSendExcessResources, value);
        }


        private DateTime _lastUpdate;

        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set => this.RaiseAndSetIfChanged(ref _lastUpdate, value);
        }

        public VillageMarket Settings { get; } = new();


        public bool IsActive { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    }
}