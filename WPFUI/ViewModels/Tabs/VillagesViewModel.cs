using DynamicData;
using DynamicData.Kernel;
using MainCore;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : AccountTabBaseViewModel
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly VillageTabStore _villageTabStore = new();

        private readonly NoVillageViewModel _noVillageViewModel;
        private readonly BuildViewModel _buildViewModel;
        private readonly VillageSettingsViewModel _villageSettingsViewModel;
        private readonly NPCViewModel _npcViewModel;
        private readonly VillageTroopsViewModel _villageTroopsViewModel;
        private readonly InfoViewModel _infoViewModel;

        public VillageTabStore VillageTabStore => _villageTabStore;

        public VillagesViewModel(SelectedItemStore SelectedItemStore, IDbContextFactory<AppDbContext> contextFactory, NoVillageViewModel noVillageViewModel, BuildViewModel buildViewModel, VillageSettingsViewModel villageSettingsViewModel, NPCViewModel npcViewModel, VillageTroopsViewModel villageTroopsViewModel, InfoViewModel infoViewModel) : base(SelectedItemStore)
        {
            _contextFactory = contextFactory;

            _noVillageViewModel = noVillageViewModel;
            _buildViewModel = buildViewModel;
            _villageSettingsViewModel = villageSettingsViewModel;
            _npcViewModel = npcViewModel;
            _villageTroopsViewModel = villageTroopsViewModel;
            _infoViewModel = infoViewModel;

            var currentVillageObservable = this.WhenAnyValue(vm => vm.CurrentVillage);
            currentVillageObservable.BindTo(_selectedItemStore, vm => vm.Village);
            currentVillageObservable.Subscribe(x =>
            {
                var tabType = TabType.Normal;
                if (x is null) tabType = TabType.NoAccount;
                VillageTabStore.SetTabType(tabType);
            });
        }

        protected override void Init(int accountId)
        {
            LoadVillageList(accountId);
        }

        private void LoadVillageList(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages
                .Where(x => x.AccountId == accountId)
                .OrderBy(x => x.Name)
                .AsList()
                .Select(x => new ListBoxItem(x.Id, x.Name, x.X, x.Y))
                .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Villages.Clear();
                Villages.AddRange(villages);
                if (villages.Any())
                {
                    CurrentVillage = Villages.First();
                }
                else
                {
                    CurrentVillage = null;
                }
            });
        }

        public ObservableCollection<ListBoxItem> Villages { get; } = new();

        private ListBoxItem _currentVillage;

        public ListBoxItem CurrentVillage
        {
            get => _currentVillage;
            set => this.RaiseAndSetIfChanged(ref _currentVillage, value);
        }

        public NoVillageViewModel NoVillageViewModel => _noVillageViewModel;
        public BuildViewModel BuildViewModel => _buildViewModel;
        public VillageSettingsViewModel VillageSettingsViewModel => _villageSettingsViewModel;
        public NPCViewModel NPCViewModel => _npcViewModel;
        public VillageTroopsViewModel VillageTroopsViewModel => _villageTroopsViewModel;
        public InfoViewModel InfoViewModel => _infoViewModel;
    }
}