using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : AccountTabBaseViewModel
    {
        public VillagesViewModel()
        {
            this.WhenAnyValue(vm => vm.CurrentVillage).BindTo(this, vm => vm._selectorViewModel.Village);
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == accountId);
            OldVillage ??= CurrentVillage;

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Villages.Clear();

                if (villages.Any())
                {
                    foreach (var village in villages)
                    {
                        Villages.Add(new()
                        {
                            Id = village.Id,
                            Name = village.Name,
                            Coords = $"{village.X}|{village.Y}",
                        });
                    }

                    var vill = Villages.FirstOrDefault(x => x.Id == OldVillage?.Id);

                    if (vill is not null) CurrentIndex = Villages.IndexOf(vill);
                    else CurrentIndex = 0;
                    OldVillage = null;
                }
            });
        }

        public ObservableCollection<VillageModel> Villages { get; } = new();

        private VillageModel _currentVillage;

        public VillageModel CurrentVillage
        {
            get => _currentVillage;
            set => this.RaiseAndSetIfChanged(ref _currentVillage, value);
        }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set => this.RaiseAndSetIfChanged(ref _currentIndex, value);
        }

        public VillageModel OldVillage { get; set; }
    }
}