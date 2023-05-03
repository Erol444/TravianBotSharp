using DynamicData;
using MainCore;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Windows.Media;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class CurrentBuildingListViewModel : ReactiveObject
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public void LoadData(int villageId)
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