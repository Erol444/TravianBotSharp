using MainCore;
using MainCore.Enums;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFUI.Models;
using WPFUI.Interfaces;
using MainCore.Helper;
using System.Windows;
using MainCore.Models.Runtime;
using MainCore.TravianData;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : ReactiveObject, IVillageTabPage
    {
        public BuildViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = App.GetService<IEventManager>();
            _planManager = App.GetService<IPlanManager>();

            NormalBuildCommand = ReactiveCommand.Create(NormalBuildTask, this.WhenAnyValue(x => x.IsLevelActive));
            ResBuildCommand = ReactiveCommand.Create(ResBuildTask);

            TopCommand = ReactiveCommand.Create(TopTask, this.WhenAnyValue(x => x.IsControlActive));
            BottomCommand = ReactiveCommand.Create(BottomTask, this.WhenAnyValue(x => x.IsControlActive));
            UpCommand = ReactiveCommand.Create(UpTask, this.WhenAnyValue(x => x.IsControlActive));
            DownCommand = ReactiveCommand.Create(DownTask, this.WhenAnyValue(x => x.IsControlActive));
            DeleteCommand = ReactiveCommand.Create(DeleteTask, this.WhenAnyValue(x => x.IsControlActive));
            DeleteAllCommand = ReactiveCommand.Create(DeleteAllTask, this.WhenAnyValue(x => x.IsControlActive));
            ImportCommand = ReactiveCommand.Create(ImportTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);

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
        }

        public void OnActived()
        {
            LoadData(VillageId);
            LoadBuildingCombo();
        }

        public void LoadData(int villageId)
        {
            LoadBuildings(villageId);
            LoadCurrent(villageId);
            LoadQueue(villageId);
        }

        private void LoadBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Id);
            Buildings.Clear();
            var queueBuildings = _planManager.GetList(villageId);

            foreach (var building in buildings)
            {
                var plannedBuild = queueBuildings.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Location == building.Id);
                if (plannedBuild is not null)
                {
                    Buildings.Add(new()
                    {
                        Location = building.Id,
                        Type = plannedBuild.Building,
                        Level = $"{building.Level} -> {plannedBuild.Level}",
                    });
                }
                else
                {
                    Buildings.Add(new()
                    {
                        Location = building.Id,
                        Type = building.Type,
                        Level = building.Level.ToString(),
                    });
                }
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
            QueueBuildings.Clear();
            var queueBuildings = _planManager.GetList(villageId);
            foreach (var building in queueBuildings)
            {
                QueueBuildings.Add(building);
            }
        }

        private void LoadBuildingCombo()
        {
            ComboBuildings.Clear();
            if (CurrentBuilding is null)
            {
                IsComboActive = false;
                IsLevelActive = false;
                return;
            }

            if (CurrentBuilding.Type != BuildingEnums.Site)
            {
                ComboBuildings.Add(new() { Building = CurrentBuilding.Type });
                SelectedBuildingIndex = 0;

                IsComboActive = false;
                IsLevelActive = true;
                return;
            }

            using var context = _contextFactory.CreateDbContext();

            var plannedBuilding = _planManager.GetList(VillageId).FirstOrDefault(x => x.Location == CurrentBuilding.Location);
            if (plannedBuilding is not null)
            {
                ComboBuildings.Add(new() { Building = plannedBuilding.Building });
                SelectedBuildingIndex = 0;

                IsComboActive = false;
                IsLevelActive = true;
                return;
            }

            var buildings = BuildingsHelper.GetCanBuild(context, _planManager, AccountId, VillageId);
            if (buildings.Count > 0)
            {
                foreach (var building in buildings)
                {
                    ComboBuildings.Add(new() { Building = building });
                }
                SelectedBuildingIndex = 0;
            }
            IsComboActive = true;
            IsLevelActive = true;
        }

        private void NormalBuildTask()
        {
            if (!NormalLevel.IsNumeric())
            {
                MessageBox.Show("Level must be numeric");
                return;
            }
            var level = NormalLevel.ToNumeric();
            if (level < 0)
            {
                MessageBox.Show("Level must be positive");
                return;
            }
            var maxLevel = BuildingsData.MaxBuildingLevel(SelectedBuilding.Building);
            if (level > maxLevel)
            {
                level = maxLevel;
            }
            var task = new PlanTask()
            {
                Level = level,
                Type = PlanTypeEnums.General,
                Building = SelectedBuilding.Building,
                Location = CurrentBuilding.Location,
            };
            _planManager.Add(VillageId, task);
            LoadQueue(VillageId);
        }

        private void ResBuildTask()
        {
            if (!ResLevel.IsNumeric())
            {
                MessageBox.Show("Level must be numeric");
                return;
            }
            var level = ResLevel.ToNumeric();
            if (level < 0)
            {
                MessageBox.Show("Level must be positive");
                return;
            }
            if (level > 20)
            {
                level = 20;
            }
            var task = new PlanTask()
            {
                Level = level,
                Type = PlanTypeEnums.ResFields,
                ResourceType = SelectedResType.Type,
                BuildingStrategy = SelectedBuildingStrategy.Strategy,
            };
            _planManager.Add(VillageId, task);
            LoadQueue(VillageId);
        }

        private void TopTask()
        {
            var index = QueueBuildings.IndexOf(CurrentQueueBuilding);
            if (index == 0) return;
            _planManager.Remove(VillageId, CurrentQueueBuilding);
            _planManager.Insert(VillageId, 0, CurrentQueueBuilding);
            LoadQueue(VillageId);
        }

        private void BottomTask()
        {
            var index = QueueBuildings.IndexOf(CurrentQueueBuilding);
            if (index == QueueBuildings.Count - 1) return;
            _planManager.Remove(VillageId, CurrentQueueBuilding);
            _planManager.Add(VillageId, CurrentQueueBuilding);
            LoadQueue(VillageId);
        }

        private void UpTask()
        {
            var index = QueueBuildings.IndexOf(CurrentQueueBuilding);
            if (index == 0) return;
            _planManager.Remove(VillageId, CurrentQueueBuilding);
            _planManager.Insert(VillageId, index - 1, CurrentQueueBuilding);
            LoadQueue(VillageId);
        }

        private void DownTask()
        {
            var index = QueueBuildings.IndexOf(CurrentQueueBuilding);
            if (index == QueueBuildings.Count - 1) return;
            _planManager.Remove(VillageId, CurrentQueueBuilding);
            _planManager.Insert(VillageId, index + 1, CurrentQueueBuilding);
            LoadQueue(VillageId);
        }

        private void DeleteTask()
        {
            _planManager.Remove(VillageId, CurrentQueueBuilding);
            LoadQueue(VillageId);
            LoadBuildings(VillageId);
        }

        private void DeleteAllTask()
        {
            _planManager.Clear(VillageId);
            LoadQueue(VillageId);
            LoadBuildings(VillageId);
        }

        private void ImportTask()
        {
        }

        private void ExportTask()
        {
        }

        public ReactiveCommand<Unit, Unit> NormalBuildCommand { get; }
        public ReactiveCommand<Unit, Unit> ResBuildCommand { get; }
        public ReactiveCommand<Unit, Unit> TopCommand { get; }
        public ReactiveCommand<Unit, Unit> BottomCommand { get; }
        public ReactiveCommand<Unit, Unit> UpCommand { get; }
        public ReactiveCommand<Unit, Unit> DownCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAllCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }

        public ObservableCollection<BuildingInfo> Buildings { get; } = new();
        public ObservableCollection<CurrentlyBuildingInfo> CurrentlyBuildings { get; } = new();
        public ObservableCollection<PlanTask> QueueBuildings { get; } = new();
        public ObservableCollection<BuildingComboBox> ComboBuildings { get; } = new();
        public ObservableCollection<ResTypeComboBox> ComboResTypes { get; } = new();
        public ObservableCollection<BuildingStrategyComboBox> ComboStrategy { get; } = new();

        private BuildingInfo _currentBuilding;

        public BuildingInfo CurrentBuilding
        {
            get => _currentBuilding;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentBuilding, value);
                LoadBuildingCombo();
            }
        }

        private PlanTask _currentQueueBuilding;

        public PlanTask CurrentQueueBuilding
        {
            get => _currentQueueBuilding;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentQueueBuilding, value);
                IsControlActive = value is not null;
            }
        }

        private BuildingComboBox _selectedBuilding;

        public BuildingComboBox SelectedBuilding
        {
            get => _selectedBuilding;
            set => this.RaiseAndSetIfChanged(ref _selectedBuilding, value);
        }

        private int _selectedBuildingIndex;

        public int SelectedBuildingIndex
        {
            get => _selectedBuildingIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedBuildingIndex, value);
        }

        private string _level;

        public string NormalLevel
        {
            get => _level;
            set => this.RaiseAndSetIfChanged(ref _level, value);
        }

        private bool _isLevelActive;

        public bool IsLevelActive
        {
            get => _isLevelActive;
            set => this.RaiseAndSetIfChanged(ref _isLevelActive, value);
        }

        private bool _isComboActive;

        public bool IsComboActive
        {
            get => _isComboActive;
            set => this.RaiseAndSetIfChanged(ref _isComboActive, value);
        }

        private bool _isControlActive;

        public bool IsControlActive
        {
            get => _isControlActive;
            set => this.RaiseAndSetIfChanged(ref _isControlActive, value);
        }

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

        private string _resLevel;

        public string ResLevel
        {
            get => _resLevel;
            set => this.RaiseAndSetIfChanged(ref _resLevel, value);
        }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly IPlanManager _planManager;

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }

        private int _villageId;

        public int VillageId
        {
            get => _villageId;
            set
            {
                this.RaiseAndSetIfChanged(ref _villageId, value);
                LoadData(value);
            }
        }
    }
}