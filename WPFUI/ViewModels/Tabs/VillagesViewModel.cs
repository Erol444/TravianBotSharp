using MainCore;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using WPFUI.Interfaces;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : ReactiveObject, IMainTabPage
    {
        public VillagesViewModel()
        {
            _databaseEvent = App.GetService<IEventManager>();
            _databaseEvent.VillagesUpdated += LoadData;
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        public void LoadData(int accountId)
        {
            var currentVillageIndex = Villages.IndexOf(CurrentVillage);
            using var context = _contextFactory.CreateDbContext();

            App.Current.Dispatcher.Invoke(() =>
            {
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
                if (currentVillageIndex != -1)
                {
                    CurrentVillage = Villages[currentVillageIndex];
                }
            });
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
                    IsVillageSelected = true;
                }
                else
                {
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

        private readonly IEventManager _databaseEvent;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set
            {
                this.RaiseAndSetIfChanged(ref _accountId, value);
                LoadData(value);
            }
        }
    }
}