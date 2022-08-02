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

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : ReactiveObject, IVillageTabPage
    {
        public BuildViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = App.GetService<IEventManager>();

            BuildCommand = ReactiveCommand.Create(BuildTask);

            TopCommand = ReactiveCommand.Create(TopTask);
            BottomCommand = ReactiveCommand.Create(BottomTask);
            UpCommand = ReactiveCommand.Create(UpTask);
            DownCommand = ReactiveCommand.Create(DownTask);
            DeleteCommand = ReactiveCommand.Create(DeleteTask);
            DeleteAllCommand = ReactiveCommand.Create(DeleteAllTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
        }

        public void OnActived()
        {
            LoadData(VillageId);
        }

        public void LoadData(int villageId)
        {
            LoadBuildings(villageId);
            LoadCurrent(villageId);
        }

        private void LoadBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Id);
            Buildings.Clear();
            foreach (var building in buildings)
            {
                Buildings.Add(new()
                {
                    Location = building.Id,
                    Type = (BuildingEnums)building.Type,
                    Level = building.Level.ToString(),
                });
            }
        }

        private void LoadCurrent(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Id);
            CurrentlyBuildings.Clear();
            foreach (var building in buildings)
            {
                CurrentlyBuildings.Add(new()
                {
                    Location = building.Id,
                    Type = (BuildingEnums)building.Type,
                    Level = building.Level,
                    CompleteTime = building.CompleteTime,
                });
            }
        }

        private void LoadQueue(int villageId)
        {
        }

        private void LoadBuildingCombo()
        {
            if (CurrentBuilding is null)
            {
                ComboBuildings.Clear();
                IsComboActive = false;
                IsLevelActive = false;
                return;
            }

            if (CurrentBuilding.Type != BuildingEnums.Site)
            {
                ComboBuildings.Clear();
                ComboBuildings.Add(CurrentBuilding);
                SelectedBuilding = CurrentBuilding;

                IsComboActive = false;
                IsLevelActive = true;
                return;
            }

            using var context = _contextFactory.CreateDbContext();

            var plannedBuilding = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Location == CurrentBuilding.Location);
            if (plannedBuilding is not null)
            {
                ComboBuildings.Clear();
                var building = new Building()
                {
                    Location = plannedBuilding.Location,
                    Type = (BuildingEnums)plannedBuilding.Type,
                };
                ComboBuildings.Add(building);
                SelectedBuilding = building;

                IsComboActive = false;
                IsLevelActive = true;
                return;
            }

            IsComboActive = true;
            IsLevelActive = true;
        }

        private void BuildTask()
        {
        }

        private void TopTask()
        {
        }

        private void BottomTask()
        {
        }

        private void UpTask()
        {
        }

        private void DownTask()
        {
        }

        private void DeleteTask()
        {
        }

        private void DeleteAllTask()
        {
        }

        private void ImportTask()
        {
        }

        private void ExportTask()
        {
        }

        public ReactiveCommand<Unit, Unit> BuildCommand { get; }
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
        public ObservableCollection<QueueBuildingInfo> QueueBuildings { get; } = new();
        public ObservableCollection<Building> ComboBuildings { get; } = new();

        private BuildingInfo _currentBuilding;

        public BuildingInfo CurrentBuilding
        {
            get => _currentBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentBuilding, value);
        }

        private QueueBuildingInfo _currentQueueBuilding;

        public QueueBuildingInfo CurrentQueueBuilding
        {
            get => _currentQueueBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentQueueBuilding, value);
        }

        private Building _selectedBuilding;

        public Building SelectedBuilding
        {
            get => _selectedBuilding;
            set => this.RaiseAndSetIfChanged(ref _selectedBuilding, value);
        }

        private string _level;

        public string Level
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

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;

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