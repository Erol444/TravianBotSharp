using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Windows.Media;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class CurrentBuildingListViewModel : VillageTabBaseViewModel
    {
        public CurrentBuildingListViewModel()
        {
            _eventManager.VillageCurrentUpdate += EventManager_VillageCurrentUpdate;
        }

        private void EventManager_VillageCurrentUpdate(int villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            LoadBuildings(villageId);
        }

        protected override void Init(int villageId)
        {
            LoadBuildings(villageId);
        }

        private void LoadBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var black = Color.FromRgb(0, 0, 0);
            var buildings = context.VillagesCurrentlyBuildings
                .Where(x => x.CompleteTime != DateTime.MaxValue && x.VillageId == villageId)
                .OrderBy(x => x.Id)
                .Select(building => new ListBoxItem(building.Id, $"{building.Type} {building.Level} complete at {building.CompleteTime}", black))
                .ToList();
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Buildings.Clear();
                Buildings.AddRange(buildings);
            });
        }

        public ObservableCollection<ListBoxItem> Buildings { get; } = new();
    }
}