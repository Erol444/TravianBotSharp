using DynamicData;
using DynamicData.Kernel;
using MainCore;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Tasks.Base;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Windows.Media;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : VillageTabBaseViewModel
    {
        private readonly IBuildingsHelper _buildingsHelper;

        public BuildViewModel() : base()
        {
            _buildingsHelper = Locator.Current.GetService<IBuildingsHelper>();

            _eventManager.VillageCurrentUpdate += EventManager_VillageUpdate;
            _eventManager.VillageBuildQueueUpdate += EventManager_VillageUpdate;
            _eventManager.VillageBuildsUpdate += EventManager_VillageUpdate;

            this.WhenAnyValue(vm => vm.CurrentBuilding).BindTo(_selectorViewModel, vm => vm.Building);
            this.WhenAnyValue(vm => vm.CurrentQueueBuilding).BindTo(_selectorViewModel, vm => vm.Queue);

            this.WhenAnyValue(vm => vm.CurrentBuilding).WhereNotNull().Subscribe(item => LoadNormalBuild(VillageId, item.Id));

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

            ResourceBuildCommand = ReactiveCommand.Create(ResourceBuildTask);
            NormalBuildCommand = ReactiveCommand.Create(NormalBuildTask, this.WhenAnyValue(vm => vm.IsLevelEnable));

            var isValid = this.WhenAnyValue(vm => vm._selectorViewModel.IsQueueSelected);
            TopCommand = ReactiveCommand.Create(TopTask, isValid);
            BottomCommand = ReactiveCommand.Create(BottomTask, isValid);
            UpCommand = ReactiveCommand.Create(UpTask, isValid);
            DownCommand = ReactiveCommand.Create(DownTask, isValid);
            DeleteCommand = ReactiveCommand.Create(DeleteTask, isValid);
            DeleteAllCommand = ReactiveCommand.Create(DeleteAllTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
        }

        #region LoadData

        protected override void Init(int villageId)
        {
            LoadData(villageId);
        }

        private void EventManager_VillageUpdate(int villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            LoadData(villageId);
        }

        private void LoadData(int villageId)
        {
            LoadBuildings(villageId);
            LoadCurrentlyBuildings(villageId);
            LoadNormalBuild(villageId, CurrentBuilding?.Id ?? -1);
            LoadQueueBuilding(villageId);

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                CurrentStrategy ??= ComboStrategy[0];
                CurrentResType ??= ComboResTypes[0];
            });
        }

        private void LoadBuildings(int villageId)
        {
            var oldIndex = -1;
            if (CurrentBuilding is not null)
            {
                oldIndex = CurrentBuilding.Id;
            }

            using var context = _contextFactory.CreateDbContext();
            var currentlyBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0).ToList();
            var queueBuildings = _planManager.GetList(villageId);
            var buildings = context.VillagesBuildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Id)
                .AsList()
                .Select(building =>
                {
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
                    return new ListBoxItem(building.Id, $"[{building.Id}] {type} | {level}", type.GetColor());
                })
                .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Buildings.Clear();
                Buildings.AddRange(buildings);
                if (buildings.Any())
                {
                    if (oldIndex == -1)
                    {
                        CurrentBuilding = buildings.First();
                    }
                    else
                    {
                        var build = buildings.FirstOrDefault(x => x.Id == oldIndex);
                        CurrentBuilding = build;
                    }
                }
            });
        }

        public void LoadCurrentlyBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var black = Color.FromRgb(0, 0, 0);

            var buildings = context.VillagesCurrentlyBuildings
               .Where(x => x.CompleteTime != DateTime.MaxValue && x.VillageId == villageId)
               .OrderBy(x => x.Id)
               .Select(building => new ListBoxItem(building.Id, $"{building.Type} - level {building.Level} complete at {building.CompleteTime}", black))
               .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                CurrentlyBuildings.Clear();
                CurrentlyBuildings.AddRange(buildings);
            });
        }

        private void LoadNormalBuild(int villageId, int location)
        {
            var (buildings, level) = GetDataNormalBuild(villageId, location);
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                NormalBuildings.Clear();
                NormalBuildings.AddRange(buildings);
                if (buildings.Count > 0)
                {
                    CurrentNormalBuilding = buildings[0];
                }
                IsComboBoxEnable = buildings.Count > 1;

                NormalLevel = level;
                IsLevelEnable = buildings.Count > 0;
            });
        }

        private (List<BuildingComboBox>, int) GetDataNormalBuild(int villageId, int location)
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

        private void LoadQueueBuilding(int villageId)
        {
            var oldIndex = -1;
            if (CurrentQueueBuilding is not null)
            {
                oldIndex = CurrentQueueBuilding.Id;
            }

            var queueBuildings = _planManager.GetList(villageId);
            var buildings = queueBuildings
                .Select(building =>
                {
                    return new ListBoxItem(queueBuildings.IndexOf(building), $"{building.Content}", Color.FromRgb(0, 0, 0));
                })
                .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                QueueBuildings.Clear();
                QueueBuildings.AddRange(buildings);
                if (buildings.Any())
                {
                    if (oldIndex == -1)
                    {
                        CurrentQueueBuilding = buildings.First();
                    }
                    else
                    {
                        var build = buildings.FirstOrDefault(x => x.Id == oldIndex);
                        CurrentQueueBuilding = build;
                    }
                }
            });
        }

        #endregion LoadData

        #region Command Handler

        private void NormalBuildTask()
        {
            var maxLevel = CurrentNormalBuilding.Building.GetMaxLevel();
            if (NormalLevel > maxLevel)
            {
                NormalLevel = maxLevel;
            }
            var planTask = new PlanTask()
            {
                Level = NormalLevel,
                Type = PlanTypeEnums.General,
                Building = CurrentNormalBuilding.Building,
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
                _taskManager.Add(accountId, _taskFactory.GetUpgradeBuildingTask(villageId, accountId));
            }
            else
            {
                task.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        private void ResourceBuildTask()
        {
            var levelMax = BuildingEnums.Woodcutter.GetMaxLevel();
            if (ResourceLevel > levelMax)
            {
                ResourceLevel = levelMax;
            }

            var planTask = new PlanTask()
            {
                Location = -1,
                Level = ResourceLevel,
                Type = PlanTypeEnums.ResFields,
                ResourceType = CurrentResType.Type,
                BuildingStrategy = CurrentStrategy.Strategy,
            };

            var villageId = VillageId;
            _planManager.Add(villageId, planTask);
            _eventManager.OnVillageBuildQueueUpdate(villageId);

            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var task = tasks.OfType<UpgradeBuilding>().FirstOrDefault(x => x.VillageId == villageId);
            if (task is null)
            {
                _taskManager.Add(accountId, _taskFactory.GetUpgradeBuildingTask(villageId, accountId));
            }
            else
            {
                task.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        private void TopTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //if (index == 0) return;
            //var villageId = VillageId;

            //var item = _planManager.GetList(villageId)[index];
            //_planManager.Remove(villageId, index);
            //_planManager.Insert(villageId, 0, item);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void BottomTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //if (index == _queueListViewModel.Buildings.Count - 1) return;
            //var villageId = VillageId;
            //var item = _planManager.GetList(villageId)[index];
            //_planManager.Remove(villageId, index);
            //_planManager.Add(villageId, item);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void UpTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //if (index == 0) return;
            //var villageId = VillageId;
            //var item = _planManager.GetList(villageId)[index];

            //_planManager.Remove(villageId, index);
            //_planManager.Insert(villageId, index - 1, item);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void DownTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //if (index == _queueListViewModel.Buildings.Count - 1) return;
            //var villageId = VillageId;
            //var item = _planManager.GetList(villageId)[index];
            //_planManager.Remove(villageId, index);
            //_planManager.Insert(villageId, index + 1, item);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void DeleteTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //var villageId = VillageId;
            //_planManager.Remove(villageId, index);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void DeleteAllTask()
        {
            //var villageId = VillageId;
            //_planManager.Clear(villageId);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void ImportTask()
        {
            //using var context = _contextFactory.CreateDbContext();
            //var accountId = AccountId;
            //var account = context.Accounts.Find(accountId);
            //var villageId = VillageId;
            //var village = context.Villages.Find(villageId);
            //var ofd = new OpenFileDialog
            //{
            //    InitialDirectory = AppContext.BaseDirectory,
            //    Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
            //    FilterIndex = 1,
            //    RestoreDirectory = true,
            //    FileName = $"{village.Name.Replace('.', '_')}_{account.Username}_queuebuildings.tbs",
            //};

            //if (ofd.ShowDialog() == true)
            //{
            //    var jsonString = File.ReadAllText(ofd.FileName);
            //    try
            //    {
            //        var queue = JsonSerializer.Deserialize<List<PlanTask>>(jsonString);
            //        foreach (var item in queue)
            //        {
            //            _planManager.Add(villageId, item);
            //        }
            //        _eventManager.OnVillageBuildQueueUpdate(villageId);
            //    }
            //    catch
            //    {
            //        MessageBox.Show("Invalid file.", "Warning");
            //        return;
            //    }
            //}
        }

        private void ExportTask()
        {
            //using var context = _contextFactory.CreateDbContext();
            //var villageId = VillageId;
            //var queueBuildings = _planManager.GetList(villageId);
            //var accountId = AccountId;
            //var account = context.Accounts.Find(accountId);
            //var village = context.Villages.Find(villageId);
            //var jsonString = JsonSerializer.Serialize(queueBuildings);
            //var svd = new SaveFileDialog
            //{
            //    InitialDirectory = AppContext.BaseDirectory,
            //    Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
            //    FilterIndex = 1,
            //    RestoreDirectory = true,
            //    FileName = $"{village.Name.Replace('.', '_')}_{account.Username}_queuebuildings.tbs",
            //};

            //if (svd.ShowDialog() == true)
            //{
            //    File.WriteAllText(svd.FileName, jsonString);
            //}
        }

        #endregion Command Handler

        #region ItemLists

        public ObservableCollection<ListBoxItem> Buildings { get; } = new();
        public ObservableCollection<ListBoxItem> CurrentlyBuildings { get; } = new();
        public ObservableCollection<BuildingComboBox> NormalBuildings { get; } = new();

        public ObservableCollection<ResTypeComboBox> ComboResTypes { get; } = new();
        public ObservableCollection<BuildingStrategyComboBox> ComboStrategy { get; } = new();
        public ObservableCollection<ListBoxItem> QueueBuildings { get; } = new();

        #endregion ItemLists

        #region ItemSelected

        private ListBoxItem _currentBuilding;

        public ListBoxItem CurrentBuilding
        {
            get => _currentBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentBuilding, value);
        }

        private BuildingComboBox _currentNormalbuilding;

        public BuildingComboBox CurrentNormalBuilding
        {
            get => _currentNormalbuilding;
            set => this.RaiseAndSetIfChanged(ref _currentNormalbuilding, value);
        }

        private ResTypeComboBox _currentResType;

        public ResTypeComboBox CurrentResType
        {
            get => _currentResType;
            set => this.RaiseAndSetIfChanged(ref _currentResType, value);
        }

        private BuildingStrategyComboBox _currentStrategy;

        public BuildingStrategyComboBox CurrentStrategy
        {
            get => _currentStrategy;
            set => this.RaiseAndSetIfChanged(ref _currentStrategy, value);
        }

        private ListBoxItem _currentQueueBuilding;

        public ListBoxItem CurrentQueueBuilding
        {
            get => _currentQueueBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentQueueBuilding, value);
        }

        #endregion ItemSelected

        #region Variable

        private int _normalLevel;

        public int NormalLevel
        {
            get => _normalLevel;
            set => this.RaiseAndSetIfChanged(ref _normalLevel, value);
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

        private int _resourceLevel;

        public int ResourceLevel
        {
            get => _resourceLevel;
            set => this.RaiseAndSetIfChanged(ref _resourceLevel, value);
        }

        #endregion Variable

        #region Command

        public ReactiveCommand<Unit, Unit> NormalBuildCommand { get; }
        public ReactiveCommand<Unit, Unit> ResourceBuildCommand { get; }
        public ReactiveCommand<Unit, Unit> TopCommand { get; }
        public ReactiveCommand<Unit, Unit> BottomCommand { get; }
        public ReactiveCommand<Unit, Unit> UpCommand { get; }
        public ReactiveCommand<Unit, Unit> DownCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAllCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }

        #endregion Command
    }
}