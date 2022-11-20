using MainCore;
using MainCore.Helper;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using UI.Models;
using UI.ViewModels.UserControls;

namespace UI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : ActivatableViewModelBase
    {
        public BuildViewModel(VillageViewModel villageViewModel, AccountViewModel accountViewModel, NormalBuildViewModel normalBuildViewModel, ResourcesBuildViewModel resourcesBuildViewModel, BuildButtonPanelViewModel buildButtonPanelViewModel, IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager)
        {
            _villageViewModel = villageViewModel;
            _accountViewModel = accountViewModel;
            _normalBuildViewModel = normalBuildViewModel;
            _resourcesBuildViewModel = resourcesBuildViewModel;
            _buildButtonPanelViewModel = buildButtonPanelViewModel;
            _contextFactory = contextFactory;
            _planManager = planManager;

            _villageViewModel.VillageChanged += OnVillageChanged;

            _normalBuildViewModel.BuildTrigger += OnBuildTrigger;
            _resourcesBuildViewModel.BuildTrigger += OnBuildTrigger;
            _buildButtonPanelViewModel.QueueChanged += OnBuildTrigger;

            this.WhenAnyValue(vm => vm.CurrentIndexBuilding).Select(x => x + 1).InvokeCommand(_normalBuildViewModel.LoadCommand);
            this.WhenAnyValue(vm => vm.CurrentIndexQueue).Subscribe(x => _buildButtonPanelViewModel.CurrentIndex = x);

            var listBuilding = new List<BuildingModel>();
            for (var i = 0; i < 40; i++)
            {
                listBuilding.Add(new BuildingModel(i));
            }
            Buildings = new(listBuilding);
        }

        protected override void OnActived(CompositeDisposable disposable)
        {
            base.OnActived(disposable);
            var villageId = _villageViewModel.Village.Id;
            LoadBuildings(villageId);
            LoadQueue(villageId);
            LoadCurrent(villageId);
        }

        protected override void OnDeactived()
        {
            base.OnDeactived();
            OldQueueBuilding = CurrentQueueBuilding;
        }

        private void OnVillageChanged(int villageId)
        {
            LoadBuildings(villageId);
            LoadQueue(villageId);
            LoadCurrent(villageId);
        }

        private void OnBuildTrigger()
        {
            var villageId = _villageViewModel.Village.Id;
            LoadBuildings(villageId);
            LoadQueue(villageId);
        }

        private void LoadBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Id);
            if (!buildings.Any()) return;

            var currentlyBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0);
            var queueBuildings = _planManager.GetList(villageId);
            var sb = new StringBuilder();
            foreach (var building in buildings)
            {
                if (building.Id < 1 || building.Id > 40) continue;
                var plannedBuild = queueBuildings.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Location == building.Id);
                var currentBuild = currentlyBuildings.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Location == building.Id);

                var type = building.Type;

                sb.Clear();
                sb.Append($"[{building.Id}] {building.Type}| Level: {building.Level}");

                if (currentBuild is not null)
                {
                    sb.Append($" -> ({currentBuild.Level})");
                    type = currentBuild.Type;
                }
                if (plannedBuild is not null)
                {
                    sb.Append($" -> [{plannedBuild.Level}]");
                    type = plannedBuild.Building;
                }

                var uiBuild = Buildings.FirstOrDefault(x => x.Id == building.Id - 1);
                uiBuild.Color = type.GetColor();
                uiBuild.Content = sb.ToString();
            }
        }

        private void LoadCurrent(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Id);
            CurrentlyBuildings.Clear();
            foreach (var building in buildings)
            {
                if (building.CompleteTime == DateTime.MaxValue) continue;
                CurrentlyBuildings.Add(new()
                {
                    Location = building.Id,
                    Type = building.Type,
                    Level = building.Level,
                    CompleteTime = building.CompleteTime,
                });
            }
        }

        private void LoadQueue(int villageId)
        {
            OldQueueBuilding ??= CurrentQueueBuilding;
            QueueBuildings.Clear();
            var queueBuildings = _planManager.GetList(villageId);
            if (queueBuildings.Any())
            {
                foreach (var building in queueBuildings)
                {
                    QueueBuildings.Add(building);
                }
                _planManager.Save();

                if (OldQueueBuilding is not null) CurrentIndexQueue = QueueBuildings.IndexOf(OldQueueBuilding);
                else CurrentIndexQueue = 0;
                OldQueueBuilding = null;
            }
        }

        private PlanTask _oldQueueBuilding;

        public PlanTask OldQueueBuilding
        {
            get => _oldQueueBuilding;
            set => this.RaiseAndSetIfChanged(ref _oldQueueBuilding, value);
        }

        private PlanTask _currentQueueBuilding;

        public PlanTask CurrentQueueBuilding
        {
            get => _currentQueueBuilding;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentQueueBuilding, value);
            }
        }

        private int _currentIndexQueue;

        public int CurrentIndexQueue
        {
            get => _currentIndexQueue;
            set => this.RaiseAndSetIfChanged(ref _currentIndexQueue, value);
        }

        private int _currentIndexBuilding;

        public int CurrentIndexBuilding
        {
            get => _currentIndexBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentIndexBuilding, value);
        }

        public ObservableCollection<BuildingModel> Buildings { get; }
        public ObservableCollection<CurrentlyBuildingModel> CurrentlyBuildings { get; } = new();
        public ObservableCollection<PlanTask> QueueBuildings { get; } = new();

        private readonly VillageViewModel _villageViewModel;
        private readonly AccountViewModel _accountViewModel;
        private readonly NormalBuildViewModel _normalBuildViewModel;
        private readonly ResourcesBuildViewModel _resourcesBuildViewModel;
        private readonly BuildButtonPanelViewModel _buildButtonPanelViewModel;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPlanManager _planManager;
    }
}