using DynamicData;
using DynamicData.Kernel;
using MainCore;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class BuildingListViewModel : ReactiveObject
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly SelectorViewModel _selectorViewModel;
        private readonly IPlanManager _planManager;

        public BuildingListViewModel()
        {
            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
            _selectorViewModel = Locator.Current.GetService<SelectorViewModel>();
            _planManager = Locator.Current.GetService<IPlanManager>();

            this.WhenAnyValue(vm => vm.CurrentBuilding).BindTo(_selectorViewModel, vm => vm.Building);
        }

        public void LoadData(int villageId)
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

        public ObservableCollection<ListBoxItem> Buildings { get; } = new();

        private ListBoxItem _currentBuilding;

        public ListBoxItem CurrentBuilding
        {
            get => _currentBuilding;
            set => this.RaiseAndSetIfChanged(ref _currentBuilding, value);
        }
    }
}