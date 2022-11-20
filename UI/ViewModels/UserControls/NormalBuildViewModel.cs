using MainCore;
using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using MainCore.Tasks.Sim;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using UI.Models;

namespace UI.ViewModels.UserControls
{
    public class NormalBuildViewModel : ViewModelBase
    {
        public NormalBuildViewModel(IPlanManager planManager, ITaskManager taskManager, IDbContextFactory<AppDbContext> contextFactory, AccountViewModel accountViewModel, VillageViewModel villageViewModel)
        {
            BuildCommand = ReactiveCommand.CreateFromTask(BuildTask, this.WhenAnyValue(x => x.IsLevelActive));
            LoadCommand = ReactiveCommand.CreateFromTask<int>(LoadTask);
            _planManager = planManager;
            _taskManager = taskManager;
            _contextFactory = contextFactory;
            _accountViewModel = accountViewModel;
            _villageViewModel = villageViewModel;

            this.WhenAnyValue(vm => vm.SelectedBuilding).Subscribe(x =>
            {
                if (ComboBuildings.Count <= 1) return;
                MinLevel = 1;
                MaxLevel = x.Building.GetMaxLevel();
            });
        }

        private Task BuildTask()
        {
            var villageId = _villageViewModel.Village.Id;
            var accountId = _accountViewModel.Account.Id;
            var planTask = new PlanTask()
            {
                Level = Level,
                Type = PlanTypeEnums.General,
                Building = SelectedBuilding.Building,
                Location = Location,
            };
            _planManager.Add(villageId, planTask);
            OnBuildTrigger();
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
            return Task.CompletedTask;
        }

        public async Task LoadTask(int location)
        {
            if (location < 0 || location > 39) return;

            if (_villageViewModel.Village is null) return;

            if (_accountViewModel.Account is null) return;

            var _location = location;
            var villageId = _villageViewModel.Village.Id;
            var accountId = _accountViewModel.Account.Id;

            var context = await _contextFactory.CreateDbContextAsync();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId);
            var currentBuilding = await buildings.FirstOrDefaultAsync(x => x.Id == location);

            ComboBuildings.Clear();
            if (currentBuilding is null)
            {
                IsComboActive = false;
                IsLevelActive = false;
            }

            if (currentBuilding.Type != BuildingEnums.Site)
            {
                ComboBuildings.Add(new() { Building = currentBuilding.Type });
                MinLevel = Level = currentBuilding.Level + 1;
                MaxLevel = currentBuilding.Type.GetMaxLevel();
                SelectedBuildingIndex = 0;
                IsComboActive = false;
                IsLevelActive = true;
                return;
            }

            var plannedBuilding = _planManager.GetList(villageId).OrderByDescending(x => x.Level).FirstOrDefault(x => x.Location == location);
            if (plannedBuilding is not null)
            {
                ComboBuildings.Add(new() { Building = plannedBuilding.Building });
                MinLevel = Level = plannedBuilding.Level + 1;
                MaxLevel = plannedBuilding.Building.GetMaxLevel();
                SelectedBuildingIndex = 0;

                IsComboActive = false;
                IsLevelActive = true;
                return;
            }
            var builableBuildings = BuildingsHelper.GetCanBuild(context, _planManager, accountId, villageId);
            if (builableBuildings.Count > 0)
            {
                foreach (var building in builableBuildings)
                {
                    ComboBuildings.Add(new() { Building = building });
                }
                Level = 1;
                SelectedBuildingIndex = 0;
            }
            IsComboActive = true;
            IsLevelActive = true;
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

        private int _level;

        public int Level
        {
            get => _level;
            set => this.RaiseAndSetIfChanged(ref _level, value);
        }

        private int _minLevel;

        public int MinLevel
        {
            get => _minLevel;
            set => this.RaiseAndSetIfChanged(ref _minLevel, value);
        }

        private int _maxLevel;

        public int MaxLevel
        {
            get => _maxLevel;
            set => this.RaiseAndSetIfChanged(ref _maxLevel, value);
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

        public event Action BuildTrigger;

        private void OnBuildTrigger() => BuildTrigger?.Invoke();

        private readonly int _location;
        public int Location => _location;
        private readonly IPlanManager _planManager;
        private readonly ITaskManager _taskManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly AccountViewModel _accountViewModel;
        private readonly VillageViewModel _villageViewModel;

        public ObservableCollection<BuildingComboBox> ComboBuildings { get; } = new();
        public ReactiveCommand<Unit, Unit> BuildCommand { get; }
        public ReactiveCommand<int, Unit> LoadCommand { get; }
    }
}