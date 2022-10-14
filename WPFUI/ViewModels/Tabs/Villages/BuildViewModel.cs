using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Runtime;
using MainCore.Tasks.Sim;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text.Json;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : VillageTabBaseViewModel, ITabPage
    {
        public BuildViewModel() : base()
        {
            NormalBuildCommand = ReactiveCommand.Create(NormalBuildTask, this.WhenAnyValue(x => x.IsLevelActive));
            ResBuildCommand = ReactiveCommand.Create(ResBuildTask);

            TopCommand = ReactiveCommand.Create(TopTask, this.WhenAnyValue(x => x.IsControlActive));
            BottomCommand = ReactiveCommand.Create(BottomTask, this.WhenAnyValue(x => x.IsControlActive));
            UpCommand = ReactiveCommand.Create(UpTask, this.WhenAnyValue(x => x.IsControlActive));
            DownCommand = ReactiveCommand.Create(DownTask, this.WhenAnyValue(x => x.IsControlActive));
            DeleteCommand = ReactiveCommand.Create(DeleteTask, this.WhenAnyValue(x => x.IsControlActive));
            DeleteAllCommand = ReactiveCommand.Create(DeleteAllTask);
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

            this.WhenAnyValue(x => x.CurrentBuilding).Subscribe(_ =>
            {
                if (CurrentVillage is not null)
                {
                    LoadBuildingCombo(CurrentVillage.Id);
                }
            });

            _eventManager.VillageBuildsUpdate += OnVillageBuildsUpdate;
            _eventManager.VillageBuildQueueUpdate += OnVillageBuildQueueUpdate;
            _eventManager.VillageCurrentUpdate += OnVillageCurrentUpdate;
        }

        private void OnVillageBuildsUpdate(int villageId)
        {
            if (!IsActive) return;
            if (CurrentVillage.Id != villageId) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadBuildings(villageId));
        }

        private void OnVillageBuildQueueUpdate(int villageId)
        {
            if (!IsActive) return;
            if (CurrentVillage.Id != villageId) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadQueue(villageId));
        }

        private void OnVillageCurrentUpdate(int villageId)
        {
            if (!IsActive) return;
            if (CurrentVillage.Id != villageId) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadCurrent(villageId));
        }

        public bool IsActive { get; set; }

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
            OldBuilding = CurrentBuilding;
            OldQueueBuilding = CurrentQueueBuilding;
        }

        protected override void LoadData(int villageId)
        {
            LoadBuildings(villageId);
            LoadCurrent(villageId);
            LoadQueue(villageId);
            LoadBuildingCombo(villageId);
        }

        private void LoadBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Id);

            OldBuilding ??= CurrentBuilding;
            Buildings.Clear();

            if (buildings.Any())
            {
                var currentlyBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0);
                var queueBuildings = _planManager.GetList(villageId);
                foreach (var building in buildings)
                {
                    if (building.Id < 1 || building.Id > 40) continue;
                    var plannedBuild = queueBuildings.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Location == building.Id);
                    var currentBuild = currentlyBuildings.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Location == building.Id);

                    var level = building.Level.ToString();
                    var type = building.Type;
                    if (currentBuild is not null)
                    {
                        level = $"{level} -> ({currentBuild.Level})";
                        type = currentBuild.Type;
                    }
                    if (plannedBuild is not null)
                    {
                        level = $"{level} -> [{plannedBuild.Level}]";
                        type = plannedBuild.Building;
                    }
                    Buildings.Add(new()
                    {
                        Location = building.Id,
                        Type = type,
                        Level = level,
                        Color = type.GetColor()
                    });
                }

                var b = Buildings.FirstOrDefault(x => x.Location == OldBuilding?.Location);
                if (b is not null) CurrentIndexBuilding = Buildings.IndexOf(b);
                else CurrentIndexBuilding = 0;
                OldBuilding = null;
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

        private void LoadBuildingCombo(int villageId)
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
                NormalLevel = CurrentBuilding.Type.GetMaxLevel().ToString();
                SelectedBuildingIndex = 0;
                IsComboActive = false;
                IsLevelActive = true;
                return;
            }

            using var context = _contextFactory.CreateDbContext();

            var plannedBuilding = _planManager.GetList(villageId).FirstOrDefault(x => x.Location == CurrentBuilding.Location);
            if (plannedBuilding is not null)
            {
                ComboBuildings.Add(new() { Building = plannedBuilding.Building });
                NormalLevel = plannedBuilding.Building.GetMaxLevel().ToString();
                SelectedBuildingIndex = 0;

                IsComboActive = false;
                IsLevelActive = true;
                return;
            }

            var buildings = BuildingsHelper.GetCanBuild(context, _planManager, CurrentAccount.Id, villageId);
            if (buildings.Count > 0)
            {
                foreach (var building in buildings)
                {
                    ComboBuildings.Add(new() { Building = building });
                }
                NormalLevel = "1";
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
            var maxLevel = SelectedBuilding.Building.GetMaxLevel();
            if (level > maxLevel)
            {
                level = maxLevel;
            }
            var planTask = new PlanTask()
            {
                Level = level,
                Type = PlanTypeEnums.General,
                Building = SelectedBuilding.Building,
                Location = CurrentBuilding.Location,
            };
            var villageId = CurrentVillage.Id;
            _planManager.Add(villageId, planTask);
            LoadQueue(villageId);
            LoadBuildings(villageId);

            var accountId = CurrentAccount.Id;
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

            var levelMax = BuildingEnums.Woodcutter.GetMaxLevel();
            if (level > levelMax)
            {
                level = levelMax;
            }
            var planTask = new PlanTask()
            {
                Location = -1,
                Level = level,
                Type = PlanTypeEnums.ResFields,
                ResourceType = SelectedResType.Type,
                BuildingStrategy = SelectedBuildingStrategy.Strategy,
            };

            var villageId = CurrentVillage.Id;
            _planManager.Add(villageId, planTask);

            LoadQueue(villageId);

            LoadBuildings(villageId);

            var accountId = CurrentAccount.Id;
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

        private void TopTask()
        {
            var index = QueueBuildings.IndexOf(CurrentQueueBuilding);
            if (index == 0) return;
            var villageId = CurrentVillage.Id;
            _planManager.Remove(villageId, CurrentQueueBuilding);
            _planManager.Insert(villageId, 0, CurrentQueueBuilding);
            LoadQueue(villageId);
        }

        private void BottomTask()
        {
            var index = QueueBuildings.IndexOf(CurrentQueueBuilding);
            if (index == QueueBuildings.Count - 1) return;
            var villageId = CurrentVillage.Id;
            _planManager.Remove(villageId, CurrentQueueBuilding);
            _planManager.Add(villageId, CurrentQueueBuilding);
            LoadQueue(villageId);
        }

        private void UpTask()
        {
            var index = QueueBuildings.IndexOf(CurrentQueueBuilding);
            if (index == 0) return;
            var villageId = CurrentVillage.Id;
            _planManager.Remove(villageId, CurrentQueueBuilding);
            _planManager.Insert(villageId, index - 1, CurrentQueueBuilding);
            LoadQueue(villageId);
        }

        private void DownTask()
        {
            var index = QueueBuildings.IndexOf(CurrentQueueBuilding);
            if (index == QueueBuildings.Count - 1) return;
            var villageId = CurrentVillage.Id;
            _planManager.Remove(villageId, CurrentQueueBuilding);
            _planManager.Insert(villageId, index + 1, CurrentQueueBuilding);
            LoadQueue(villageId);
        }

        private void DeleteTask()
        {
            var villageId = CurrentVillage.Id;
            _planManager.Remove(villageId, CurrentQueueBuilding);
            LoadQueue(villageId);
            LoadBuildings(villageId);
        }

        private void DeleteAllTask()
        {
            var villageId = CurrentVillage.Id;
            _planManager.Clear(villageId);
            LoadQueue(villageId);
            LoadBuildings(villageId);
        }

        private void ImportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var accountId = CurrentAccount.Id;
            var account = context.Accounts.Find(accountId);
            var villageId = CurrentVillage.Id;
            var village = context.Villages.Find(villageId);
            var ofd = new OpenFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{village.Name.Replace('.', '_')}_{account.Username}_queuebuildings.tbs",
            };

            if (ofd.ShowDialog() == true)
            {
                var jsonString = File.ReadAllText(ofd.FileName);
                try
                {
                    var queue = JsonSerializer.Deserialize<List<PlanTask>>(jsonString);
                    foreach (var item in queue)
                    {
                        _planManager.Add(villageId, item);
                    }
                    LoadQueue(villageId);
                }
                catch
                {
                    MessageBox.Show("Invalid file.", "Warning");
                    return;
                }
            }
        }

        private void ExportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageId = CurrentVillage.Id;
            var queueBuildings = _planManager.GetList(villageId);
            var accountId = CurrentAccount.Id;
            var account = context.Accounts.Find(accountId);
            var village = context.Villages.Find(villageId);
            var jsonString = JsonSerializer.Serialize(queueBuildings);
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{village.Name.Replace('.', '_')}_{account.Username}_queuebuildings.tbs",
            };

            if (svd.ShowDialog() == true)
            {
                File.WriteAllText(svd.FileName, jsonString);
            }
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

        private BuildingInfo _oldBuilding;

        public BuildingInfo OldBuilding
        {
            get => _oldBuilding;
            set => this.RaiseAndSetIfChanged(ref _oldBuilding, value);
        }

        private BuildingInfo _currentBuilding;

        public BuildingInfo CurrentBuilding
        {
            get => _currentBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentBuilding, value);
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

        private int _currentIndexBuilding;

        public int CurrentIndexBuilding
        {
            get => _currentIndexBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentIndexBuilding, value);
        }

        private int _currentIndexQueue;

        public int CurrentIndexQueue
        {
            get => _currentIndexQueue;
            set => this.RaiseAndSetIfChanged(ref _currentIndexQueue, value);
        }
    }
}