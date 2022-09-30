using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : AccountTabBaseViewModel, ITabPage
    {
        public VillagesViewModel() : base()
        {
            _eventManager.VillagesUpdated += OnVillagesUpdate;

            _isVillageSelected = this.WhenAnyValue(x => x.CurrentVillage).Select(x => x is not null).ToProperty(this, x => x.IsVillageSelected);
            _isVillageNotSelected = this.WhenAnyValue(x => x.CurrentVillage).Select(x => x is null).ToProperty(this, x => x.IsVillageNotSelected);
        }

        public bool IsActive { get; set; }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentAccount is not null)
            {
                LoadData(CurrentAccount.Id);
            }
        }

        public void OnDeactived()
        {
            IsActive = false;
        }

        public void OnVillagesUpdate(int accountId)
        {
            if (!IsActive) return;
            if (CurrentAccount is null) return;
            if (CurrentAccount.Id != accountId) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadData(accountId));
        }

        protected override void LoadData(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == accountId);
            Villages.Clear();
            foreach (var village in villages)
            {
                Villages.Add(new()
                {
                    Id = village.Id,
                    Name = village.Name,
                    Coords = $"{village.X}|{village.Y}",
                });
            }
        }

        public ObservableCollection<Village> Villages { get; } = new();

        private Village _currentVillage;

        public Village CurrentVillage
        {
            get => _currentVillage;
            set => this.RaiseAndSetIfChanged(ref _currentVillage, value);
        }

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