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

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : ReactiveObject, IVillageTabPage
    {
        public BuildViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = App.GetService<IEventManager>();

            BuildCommand = ReactiveCommand.Create(BuildTask, this.WhenAnyValue(x => x.IsLevelActive));

            TopCommand = ReactiveCommand.Create(TopTask, this.WhenAnyValue(x => x.IsControlActive));
            BottomCommand = ReactiveCommand.Create(BottomTask, this.WhenAnyValue(x => x.IsControlActive));
            UpCommand = ReactiveCommand.Create(UpTask, this.WhenAnyValue(x => x.IsControlActive));
            DownCommand = ReactiveCommand.Create(DownTask, this.WhenAnyValue(x => x.IsControlActive));
            DeleteCommand = ReactiveCommand.Create(DeleteTask, this.WhenAnyValue(x => x.IsControlActive));
            DeleteAllCommand = ReactiveCommand.Create(DeleteAllTask, this.WhenAnyValue(x => x.IsControlActive));
            ImportCommand = ReactiveCommand.Create(ImportTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
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
            foreach (var building in buildings)
            {
                Buildings.Add(new()
                {
                    Location = building.Id,
                    Type = building.Type,
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
                    Type = building.Type,
                    Level = building.Level,
                    CompleteTime = building.CompleteTime,
                });
            }
        }

        private void LoadQueue(int villageId)
        {
            QueueBuildings.Clear();
            using var context = _contextFactory.CreateDbContext();
            var queueBuildings = context.VillagesQueueBuildings.Where(x => x.VillageId == villageId).OrderBy(x => x.Item);
            foreach (var building in queueBuildings)
            {
                QueueBuildings.Add(new()
                {
                    Id = building.Id,
                    Location = building.Location,
                    Type = building.Type,
                    Level = building.Level,
                });
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

            var plannedBuilding = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Location == CurrentBuilding.Location);
            if (plannedBuilding is not null)
            {
                ComboBuildings.Add(new() { Building = plannedBuilding.Type });
                SelectedBuildingIndex = 0;

                IsComboActive = false;
                IsLevelActive = true;
                return;
            }

            var buildings = BuildingsHelper.GetCanBuild(context, AccountId, VillageId);
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

        private void BuildTask()
        {
            if (!Level.IsNumeric())
            {
                MessageBox.Show("Level must be numeric");
                return;
            }
            using var context = _contextFactory.CreateDbContext();
            context.VillagesQueueBuildings.Add(new()
            {
                Item = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Count(),
                VillageId = VillageId,
                Level = Level.ToNumeric(),
                Type = SelectedBuilding.Building,
                Location = CurrentBuilding.Location,
            });
            context.SaveChanges();
            LoadQueue(VillageId);
        }

        private void TopTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Count();
            if (count < 2)
            {
                return;
            }
            var current = context.VillagesQueueBuildings.Find(CurrentQueueBuilding.Id);
            var others = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Item < current.Item);

            current.Item = 0;

            foreach (var other in others)
            {
                other.Item++;
            }
            context.SaveChanges();
            LoadQueue(VillageId);
        }

        private void BottomTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Count();
            if (count < 2)
            {
                return;
            }
            var current = context.VillagesQueueBuildings.Find(CurrentQueueBuilding.Id);
            var others = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Item > current.Item);

            current.Item = count-1;

            foreach (var other in others)
            {
                other.Item--;
            }
            context.SaveChanges();
            LoadQueue(VillageId);
        }

        private void UpTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Count();
            if (count < 2)
            {
                return;
            }
            var current = context.VillagesQueueBuildings.Find(CurrentQueueBuilding.Id);
            if (current.Item == 0)
            {
                return;
            }
            var other = context.VillagesQueueBuildings.FirstOrDefault(x => x.Item == current.Item - 1);

            var temp = current.Item;
            current.Item = other.Item;
            other.Item = temp;
            context.SaveChanges();
            LoadQueue(VillageId);
        }

        private void DownTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Count();
            if (count < 2)
            {
                return;
            }
            var current = context.VillagesQueueBuildings.Find(CurrentQueueBuilding.Id);
            if (current.Item == count - 1)
            {
                return;
            }
            var other = context.VillagesQueueBuildings.FirstOrDefault(x => x.Item == current.Item + 1);

            var temp = current.Item;
            current.Item = other.Item;
            other.Item = temp;
            context.SaveChanges();
            LoadQueue(VillageId);
        }

        private void DeleteTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var current = context.VillagesQueueBuildings.Find(VillageId, CurrentQueueBuilding.Id);
            var count = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Count();
            if (count < 2)
            {
                context.Remove(current);
            }
            else
            {
                var others = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Item > current.Item);
                foreach (var other in others)
                {
                    other.Item--;
                }
                context.Remove(current);
            }
            context.SaveChanges();
            LoadQueue(VillageId);
        }

        private void DeleteAllTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var plannedBuild = context.VillagesQueueBuildings.Where(x => x.VillageId == VillageId);
            context.RemoveRange(plannedBuild);
            context.SaveChanges();
            LoadQueue(VillageId);
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
        public ObservableCollection<BuildingComboBox> ComboBuildings { get; } = new();

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

        private QueueBuildingInfo _currentQueueBuilding;

        public QueueBuildingInfo CurrentQueueBuilding
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

        private bool _isControlActive;

        public bool IsControlActive
        {
            get => _isControlActive;
            set => this.RaiseAndSetIfChanged(ref _isControlActive, value);
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