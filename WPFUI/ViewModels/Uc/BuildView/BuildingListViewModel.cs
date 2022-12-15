using DynamicData;
using DynamicData.Kernel;
using MainCore.Helper;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class BuildingListViewModel : VillageTabBaseViewModel
    {
        public BuildingListViewModel()
        {
            this.WhenAnyValue(vm => vm.CurrentBuilding).BindTo(_selectorViewModel, vm => vm.Building);
            LoadCommand = ReactiveCommand.CreateFromTask(BuildTask, this.WhenAnyValue(vm => vm._selectorViewModel.IsVillageSelected));
            _eventManager.VillageCurrentUpdate += EventManager_VillageCurrentUpdate;
        }

        protected override void Init(int villageId)
        {
            LoadBuildings(villageId);
        }

        private void EventManager_VillageCurrentUpdate(int villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            LoadBuildings(villageId);
        }

        private Task BuildTask()
        {
            if (!IsActive) return Task.CompletedTask;
            return Task.Run(() => LoadBuildings(VillageId));
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
                        CurrentBuilding = Buildings.First();
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

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
    }
}