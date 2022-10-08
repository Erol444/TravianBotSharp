using MainCore.Models.Database;
using ReactiveUI;
using System.Reactive;
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
            Resources = new()
            {
                Warehouse = 0,
                Wood = 1000000000000000000,
                Clay = 1000,
                Iron = 1000,
                Granary = 0,
                Crop = 0,
            };
            Ratio = new()
            {
                Wood = "1000",
                Clay = "1000",
                Iron = "1000",
                Crop = "1000",
            };
        }

        private void RefreshTask()
        {
        }

        private void NPCTask()
        {
        }

        public void OnActived()
        {
            if (CurrentVillage is null) return;
            LoadData(CurrentVillage.Id);
        }

        public void OnDeactived()
        {
        }

        private VillageResources _resources;

        public VillageResources Resources
        {
            get => _resources;
            set => this.RaiseAndSetIfChanged(ref _resources, value);
        }

        private Resources _ratio;

        public Resources Ratio
        {
            get => _ratio;
            set => this.RaiseAndSetIfChanged(ref _ratio, value);
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }
        public ReactiveCommand<Unit, Unit> NPCCommand { get; set; }
        public bool IsActive { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}