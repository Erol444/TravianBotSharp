using MainCore;
using MainCore.Enums;
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

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : ReactiveObject
    {
        public BuildViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();

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

        public void LoadData(int villageId)
        {
            LoadBuildings(villageId);
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
                    Color = Color.FromRgb(0, 0, 0),
                });
            }
        }

        private void LoadCurrent(int villageId)
        {
        }

        private void LoadQueue(int villageId)
        {
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
        public ObservableCollection<Building> SelectorBuildings { get; } = new();

        private Building _currentBuilding;

        public Building CurrentBuilding
        {
            get => _currentBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentBuilding, value);
        }

        private Building _currentQueueBuilding;

        public Building CurrentQueueBuilding
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

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}