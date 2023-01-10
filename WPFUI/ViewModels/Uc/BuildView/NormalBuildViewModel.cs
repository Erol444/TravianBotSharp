using DynamicData;
using MainCore;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Tasks.Sim;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class NormalBuildViewModel : VillageTabBaseViewModel
    {
        private readonly IBuildingsHelper _buildingsHelper;

        public NormalBuildViewModel()
        {
            BuildCommand = ReactiveCommand.Create(BuildTask, this.WhenAnyValue(vm => vm.IsLevelEnable));
            _buildingsHelper = Locator.Current.GetService<IBuildingsHelper>();
            _selectorViewModel.BuildingChanged += SelectorViewModel_BuildingChanged;
        }

        private void SelectorViewModel_BuildingChanged(int location)
        {
            if (_selectorViewModel.IsVillageNotSelected) return;
            LoadData(VillageId, location);
        }

        protected override void Init(int villageid)
        {
            LoadData(villageid, _selectorViewModel.Building?.Id ?? -1);
        }

        private void LoadData(int villageId, int location)
        {
            var (buildings, level) = GetData(villageId, location);
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Buildings.Clear();
                Buildings.AddRange(buildings);
                if (buildings.Count > 0)
                {
                    Building = buildings[0];
                }
                IsComboBoxEnable = buildings.Count > 1;

                Level = level;
                MinimumLevel = level;
                IsLevelEnable = buildings.Count > 0;
            });
        }

        private (List<BuildingComboBox>, int) GetData(int villageId, int location)
        {
            if (location == -1) return (new(), 0);

            using var context = _contextFactory.CreateDbContext();

            var currentBuilding = context.VillagesBuildings.FirstOrDefault(x => x.VillageId == villageId && x.Id == location);
            if (currentBuilding is not null && currentBuilding.Type != BuildingEnums.Site)
            {
                return (new() { new() { Building = currentBuilding.Type } }, currentBuilding.Level + 1);
            }

            var plannedBuilding = _planManager.GetList(villageId).FirstOrDefault(x => x.Location == location);
            if (plannedBuilding is not null)
            {
                return (new() { new() { Building = plannedBuilding.Building } }, plannedBuilding.Level + 1);
            }

            var buildings = _buildingsHelper.GetCanBuild(AccountId, villageId);
            if (buildings.Count > 0)
            {
                var list = buildings.Select(x => new BuildingComboBox() { Building = x }).ToList();
                return (list, 1);
            }
            return (new(), 0);
        }

        private void BuildTask()
        {
            var maxLevel = Building.Building.GetMaxLevel();
            if (Level > maxLevel)
            {
                Level = maxLevel;
            }
            var planTask = new PlanTask()
            {
                Level = Level,
                Type = PlanTypeEnums.General,
                Building = Building.Building,
                Location = _selectorViewModel.Building.Id,
            };
            var villageId = VillageId;
            _planManager.Add(villageId, planTask);
            _eventManager.OnVillageBuildQueueUpdate(villageId);
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var task = tasks.OfType<UpgradeBuilding>().FirstOrDefault(x => x.VillageId == villageId);
            if (task is null)
            {
                _taskManager.Add(accountId, new UpgradeBuilding(villageId, accountId));
            }
            else
            {
                task.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        public ReactiveCommand<Unit, Unit> BuildCommand { get; }
        public ObservableCollection<BuildingComboBox> Buildings { get; } = new();

        private BuildingComboBox _building;

        public BuildingComboBox Building
        {
            get => _building;
            set => this.RaiseAndSetIfChanged(ref _building, value);
        }

        private int _level;

        public int Level
        {
            get => _level;
            set => this.RaiseAndSetIfChanged(ref _level, value);
        }

        private int _minimumLevel;

        public int MinimumLevel
        {
            get => _minimumLevel;
            set => this.RaiseAndSetIfChanged(ref _minimumLevel, value);
        }

        private bool _isLevelEnable;

        public bool IsLevelEnable
        {
            get => _isLevelEnable;
            set => this.RaiseAndSetIfChanged(ref _isLevelEnable, value);
        }

        private bool _isComboBoxEnable;

        public bool IsComboBoxEnable
        {
            get => _isComboBoxEnable;
            set => this.RaiseAndSetIfChanged(ref _isComboBoxEnable, value);
        }
    }
}