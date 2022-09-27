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
    public class VillagesViewModel : AccountTabBaseViewModel, IMainTabPage
    {
        public VillagesViewModel() : base()
        {
            _eventManager.VillagesUpdated += OnVillagesUpdate;

            _isVillageSelected = this.WhenAnyValue(x => x.CurrentVillage).Select(x => x is not null).ToProperty(this, x => x.IsVillageSelected);
            _isVillageNotSelected = this.WhenAnyValue(x => x.CurrentVillage).Select(x => x is null).ToProperty(this, x => x.IsVillageNotSelected);
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        public void OnVillagesUpdate(int accountId)
        {
            if (accountId != AccountId) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadData(AccountId));
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

        public ObservableCollection<VillageInfo> Villages { get; } = new();

        private VillageInfo _currentVillage;

        public VillageInfo CurrentVillage
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