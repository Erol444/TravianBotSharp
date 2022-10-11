using MainCore.Models.Database;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Update;
using ReactiveUI;
using System;
using System.Reactive;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class NPCViewModel : VillageTabBaseViewModel, ITabPage
    {
        public NPCViewModel() : base()
        {
            RefreshCommand = ReactiveCommand.Create(RefreshTask);
            NPCCommand = ReactiveCommand.Create(NPCTask);
        }

        protected override void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            Resources = context.VillagesResources.Find(index);
            var updateTime = context.VillagesUpdateTime.Find(index);
            var dorf1 = updateTime.Dorf1;
            var dorf2 = updateTime.Dorf2;
            LastUpdate = dorf1 > dorf2 ? dorf1 : dorf2;
        }

        private void RefreshTask()
        {
            _taskManager.Add(CurrentAccount.Id, new UpdateVillage(CurrentVillage.Id, CurrentAccount.Id));
            MessageBox.Show("Added Refresh resources task to queue");
        }

        private void NPCTask()
        {
            _taskManager.Add(CurrentAccount.Id, new NPCTask(CurrentVillage.Id, CurrentAccount.Id, Ratio.GetResources()));
            MessageBox.Show("Added NPC task to queue");
        }

        public void OnActived()
        {
            if (CurrentVillage is null) return;
            IsActive = true;
            LoadData(CurrentVillage.Id);
            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.VillagesSettings.Find(CurrentVillage.Id);
                Ratio.Wood = setting.AutoNPCWood.ToString();
                Ratio.Clay = setting.AutoNPCClay.ToString();
                Ratio.Iron = setting.AutoNPCIron.ToString();
                Ratio.Crop = setting.AutoNPCCrop.ToString();
            }
        }

        public void OnDeactived()
        {
            IsActive = false;
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
        public bool IsActive { get; set; }
    }
}