using MainCore;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using MainCore.Tasks.FunctionTasks;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using WPFUI.Models;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class NPCViewModel : VillageTabBaseViewModel
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ITaskManager _taskManager;

        public NPCViewModel(SelectedItemStore selectedItemStore, IDbContextFactory<AppDbContext> contextFactory, ITaskManager taskManager) : base(selectedItemStore)
        {
            _contextFactory = contextFactory;
            _taskManager = taskManager;

            RefreshCommand = ReactiveCommand.Create(RefreshTask);
            NPCCommand = ReactiveCommand.Create(NPCTask);
        }

        protected override void Init(int villageId)
        {
            LoadData(villageId);
        }

        private void LoadData(int villageId)
        {
            Observable.Start(() =>
            {
                using var context = _contextFactory.CreateDbContext();
                var resources = context.VillagesResources.Find(villageId);
                var updateTime = context.VillagesUpdateTime.Find(villageId);
                var setting = context.VillagesSettings.Find(VillageId);
                return (resources, updateTime, setting);
            }, RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((data) =>
                {
                    var (resources, updateTime, setting) = data;
                    var dorf1 = updateTime.Dorf1;
                    var dorf2 = updateTime.Dorf2;

                    Resources = resources;
                    LastUpdate = dorf1 > dorf2 ? dorf1 : dorf2;

                    Ratio.Wood = setting.AutoNPCWood.ToString();
                    Ratio.Clay = setting.AutoNPCClay.ToString();
                    Ratio.Iron = setting.AutoNPCIron.ToString();
                    Ratio.Crop = setting.AutoNPCCrop.ToString();
                });
        }

        private void RefreshTask()
        {
            _taskManager.Add<RefreshVillage>(AccountId, VillageId);
            MessageBox.Show("Added Refresh resources task to queue");
        }

        private void NPCTask()
        {
            _taskManager.Add<NPCTask>(AccountId, () => new(VillageId, AccountId, Ratio.GetResources()));
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