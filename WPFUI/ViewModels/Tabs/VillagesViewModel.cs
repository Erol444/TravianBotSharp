using MainCore;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : ReactiveObject
    {
        public VillagesViewModel()
        {
            _databaseEvent = App.GetService<IDatabaseEvent>();
            _databaseEvent.AccountSelected += LoadData;
            _databaseEvent.VillagesUpdated += LoadData;
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
        }

        public void LoadData(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == accountId);
            Villages.Clear();
            foreach (var village in villages)
            {
                Villages.Add(new VillageInfo()
                {
                    Id = village.Id,
                    Name = village.Name,
                    Coords = $"{village.X}|{village.Y}",
                });
            }
        }

        private VillageInfo _currentVillage;

        public VillageInfo CurrentVillage
        {
            get => _currentVillage;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentVillage, value);
                if (value is not null)
                {
                    _databaseEvent.OnVillageSelected(value.Id);
                    IsVillageSelected = true;
                }
                else
                {
                    _databaseEvent.OnVillageSelected(-1);
                    IsVillageSelected = false;
                }
            }
        }

        private bool _isVillageSelected = false;

        public bool IsVillageSelected
        {
            get => _isVillageSelected;
            set
            {
                this.RaiseAndSetIfChanged(ref _isVillageSelected, value);
                IsVillageNotSelected = !value;
            }
        }

        private bool _isVillageNotSelected = true;

        public bool IsVillageNotSelected
        {
            get => _isVillageNotSelected;
            set => this.RaiseAndSetIfChanged(ref _isVillageNotSelected, value);
        }

        public ObservableCollection<VillageInfo> Villages { get; } = new();

        private readonly IDatabaseEvent _databaseEvent;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}