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
    public class ResourcesBuildViewModel : ViewModelBase
    {
        public ResourcesBuildViewModel(IPlanManager planManager, ITaskManager taskManager, IDbContextFactory<AppDbContext> contextFactory, AccountViewModel accountViewModel, VillageViewModel villageViewModel)
        {
            _planManager = planManager;
            _taskManager = taskManager;
            _contextFactory = contextFactory;
            _accountViewModel = accountViewModel;
            _villageViewModel = villageViewModel;
            BuildCommand = ReactiveCommand.CreateFromTask(BuildTask);
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

            MinLevel = 1;
            MaxLevel = BuildingEnums.Woodcutter.GetMaxLevel();
        }

        private Task BuildTask()
        {
            var villageId = _villageViewModel.Village.Id;
            var accountId = _accountViewModel.Account.Id;
            var planTask = new PlanTask()
            {
                Location = -1,
                Level = Level,
                Type = PlanTypeEnums.ResFields,
                ResourceType = SelectedResType.Type,
                BuildingStrategy = SelectedStrategy.Strategy,
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
            get => _minLevel;
            set => this.RaiseAndSetIfChanged(ref _maxLevel, value);
        }

        private ResourceTypeComboBoxModel _selectedResType;

        public ResourceTypeComboBoxModel SelectedResType
        {
            get => _selectedResType;
            set => this.RaiseAndSetIfChanged(ref _selectedResType, value);
        }

        private StrategyComboBoxModel _selectedStrategy;

        public StrategyComboBoxModel SelectedStrategy
        {
            get => _selectedStrategy;
            set => this.RaiseAndSetIfChanged(ref _selectedStrategy, value);
        }

        public event Action BuildTrigger;

        private void OnBuildTrigger() => BuildTrigger?.Invoke();

        public ReactiveCommand<Unit, Unit> BuildCommand { get; }
        public ReactiveCommand<int, Unit> LoadCommand { get; }

        public ObservableCollection<ResourceTypeComboBoxModel> ComboResTypes { get; } = new();
        public ObservableCollection<StrategyComboBoxModel> ComboStrategy { get; } = new();

        private readonly IPlanManager _planManager;
        private readonly ITaskManager _taskManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly AccountViewModel _accountViewModel;
        private readonly VillageViewModel _villageViewModel;
    }
}