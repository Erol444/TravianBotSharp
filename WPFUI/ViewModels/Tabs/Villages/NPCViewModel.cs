using MainCore.Models.Database;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Update;
using ReactiveUI;
using System;
using System.Reactive;
using System.Windows;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class NPCViewModel : VillageTabViewModelBase
    {
        public NPCViewModel()
        {
            RefreshCommand = ReactiveCommand.Create(RefreshTask);
            NPCCommand = ReactiveCommand.Create(NPCTask);
        }

        protected override void Init(int villageId)
        {
            LoadData(villageId);
        }

        private void LoadData(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            Resources = context.VillagesResources.Find(villageId);
            var updateTime = context.VillagesUpdateTime.Find(villageId);
            var dorf1 = updateTime.Dorf1;
            var dorf2 = updateTime.Dorf2;
            LastUpdate = dorf1 > dorf2 ? dorf1 : dorf2;

            var setting = context.VillagesSettings.Find(VillageId);
            Ratio.Wood = setting.AutoNPCWood.ToString();
            Ratio.Clay = setting.AutoNPCClay.ToString();
            Ratio.Iron = setting.AutoNPCIron.ToString();
            Ratio.Crop = setting.AutoNPCCrop.ToString();
        }

        private void RefreshTask()
        {
            _taskManager.Add(AccountId, new UpdateVillage(VillageId, AccountId));
            MessageBox.Show("Added Refresh resources task to queue");
        }

        private void NPCTask()
        {
            _taskManager.Add(AccountId, new NPCTask(VillageId, AccountId, Ratio.GetResources()));
            MessageBox.Show("Added NPC task to queue");
        }

        private VillageResources _resources;

        public VillageResources Resources
        {
            get => _resources;
            set => this.RaiseAndSetIfChanged(ref _resources, value);
        }

        private Resources _ratio = new();

        public Resources Ratio
        {
            get => _ratio;
            set => this.RaiseAndSetIfChanged(ref _ratio, value);
        }

        private DateTime _lastUpdate;

        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set => this.RaiseAndSetIfChanged(ref _lastUpdate, value);
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }
        public ReactiveCommand<Unit, Unit> NPCCommand { get; set; }
    }
}