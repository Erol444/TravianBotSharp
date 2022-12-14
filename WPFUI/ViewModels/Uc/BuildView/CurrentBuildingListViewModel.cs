using DynamicData;
using MainCore.Helper;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class CurrentBuildingListViewModel : VillageTabBaseViewModel
    {
        protected override void Init(int villageId)
        {
            LoadBuildings(villageId);
        }

        private void LoadBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesCurrentlyBuildings
                .Where(x => x.CompleteTime != DateTime.MaxValue && x.VillageId == villageId)
                .OrderBy(x => x.Id)
                .Select(building => new ListBoxItem(building.Id, $"[{building.Id}] {building.Type} {building.Level} complete at {building.CompleteTime}", building.Type.GetColor()))
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