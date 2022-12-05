using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Interfaces;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : ActivatableViewModelBase, ITabPage
    {
        public VillagesViewModel() : base()
        {
            _eventManager.VillagesUpdated += OnVillagesUpdate;
        }

        public bool IsActive { get; set; }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentAccount is not null)
            {
                LoadData(AccountId);
            }
        }

        public void OnDeactived()
        {
            IsActive = false;
            OldVillage = CurrentVillage;
        }

        public void OnVillagesUpdate(int accountId)
        {
            if (!IsActive) return;
            if (CurrentAccount is null) return;
            if (AccountId != accountId) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadData(accountId));
        }

        protected override void LoadData(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == accountId);
            OldVillage ??= CurrentVillage;

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

        private readonly ObservableAsPropertyHelper<bool> _isVillageSelected;

        public bool IsVillageSelected
        {
            get => _isVillageSelected.Value;
        }

        private readonly ObservableAsPropertyHelper<bool> _isVillageNotSelected;

        public bool IsVillageNotSelected
        {
            get => _isVillageNotSelected.Value;
        }
    }
}