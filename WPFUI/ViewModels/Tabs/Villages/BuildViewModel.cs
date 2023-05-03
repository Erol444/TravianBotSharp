using DynamicData;
using DynamicData.Kernel;
using MainCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Windows.Media;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : VillageTabBaseViewModel
    {
        public BuildViewModel() : base()
        {
            _eventManager.VillageCurrentUpdate += EventManager_VillageUpdate;
            _eventManager.VillageBuildQueueUpdate += EventManager_VillageUpdate;
            _eventManager.VillageBuildsUpdate += EventManager_VillageUpdate;
        }

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

        public ObservableCollection<ListBoxItem> Buildings { get; } = new();
        public ObservableCollection<ListBoxItem> CurrentlyBuildings { get; } = new();

        private ListBoxItem _currentBuilding;

        public ListBoxItem CurrentBuilding
        {
            get => _currentBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentBuilding, value);
        }
    }
}