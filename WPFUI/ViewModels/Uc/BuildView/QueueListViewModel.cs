using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Windows.Media;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class QueueListViewModel : VillageTabBaseViewModel
    {
        public QueueListViewModel()
        {
            this.WhenAnyValue(vm => vm.CurrentItem).BindTo(_selectorViewModel, vm => vm.Queue);
            _eventManager.VillageBuildQueueUpdate += EventManager_VillageBuildQueueUpdate;
        }

        private void EventManager_VillageBuildQueueUpdate(int villageId)
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
            var oldIndex = -1;
            if (CurrentItem is not null)
            {
                oldIndex = CurrentItem.Id;
            }

            var queueBuildings = _planManager.GetList(villageId);
            var buildings = queueBuildings
                .Select(building =>
                {
                    return new ListBoxItem(queueBuildings.IndexOf(building), $"{building.Content}", Color.FromRgb(0, 0, 0));
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
                        CurrentItem = Buildings.First();
                    }
                    else
                    {
                        var build = buildings.FirstOrDefault(x => x.Id == oldIndex);
                        CurrentItem = build;
                    }
                }
            });
        }

        public ObservableCollection<ListBoxItem> Buildings { get; } = new();

        private ListBoxItem _currentItem;

        public ListBoxItem CurrentItem
        {
            get => _currentItem;
            set => this.RaiseAndSetIfChanged(ref _currentItem, value);
        }
    }
}