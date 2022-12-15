using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Runtime;
using MainCore.Tasks.Sim;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class ResourcesBuildViewModel : VillageTabBaseViewModel
    {
        public ResourcesBuildViewModel()
        {
            foreach (var item in Enum.GetValues(typeof(ResTypeEnums)))
            {
                ComboResTypes.Add(new()
                {
                    Type = (ResTypeEnums)item,
                });
            }

            foreach (var item in Enum.GetValues(typeof(BuildingStrategyEnums)))
            {
                ComboStrategy.Add(new()
                {
                    Strategy = (BuildingStrategyEnums)item,
                });
            }
            Level = 1;

            BuildCommand = ReactiveCommand.Create(BuildTask);
        }

        protected override void Init(int id)
        {
            SelectedBuildingStrategy ??= ComboStrategy[0];
            SelectedResType ??= ComboResTypes[0];
        }

        private void BuildTask()
        {
            var levelMax = BuildingEnums.Woodcutter.GetMaxLevel();
            if (Level > levelMax)
            {
                Level = levelMax;
            }

            var planTask = new PlanTask()
            {
                Location = -1,
                Level = Level,
                Type = PlanTypeEnums.ResFields,
                ResourceType = SelectedResType.Type,
                BuildingStrategy = SelectedBuildingStrategy.Strategy,
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

        public ObservableCollection<ResTypeComboBox> ComboResTypes { get; } = new();
        public ObservableCollection<BuildingStrategyComboBox> ComboStrategy { get; } = new();
        public ReactiveCommand<Unit, Unit> BuildCommand { get; }

        private ResTypeComboBox _selectedResType;

        public ResTypeComboBox SelectedResType
        {
            get => _selectedResType;
            set => this.RaiseAndSetIfChanged(ref _selectedResType, value);
        }

        private BuildingStrategyComboBox _selectedBuildingStrategy;

        public BuildingStrategyComboBox SelectedBuildingStrategy
        {
            get => _selectedBuildingStrategy;
            set => this.RaiseAndSetIfChanged(ref _selectedBuildingStrategy, value);
        }

        private int _level;

        public int Level
        {
            get => _level;
            set => this.RaiseAndSetIfChanged(ref _level, value);
        }
    }
}