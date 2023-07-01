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
        private readonly VillageNavigationStore _villageNavigationStore;
        private readonly TabType _currentTab;

        private readonly ObservableAsPropertyHelper<ViewModelBase> _currentViewModel;

        public ObservableCollection<ListBoxItem> Villages { get; } = new();

        private ListBoxItem _currentVillage;

        public VillagesViewModel(SelectedItemStore SelectedItemStore, VillageNavigationStore villageNavigationStore, IDbContextFactory<AppDbContext> contextFactory, NoVillageViewModel noVillageViewModel, BuildViewModel buildViewModel, VillageSettingsViewModel villageSettingsViewModel, NPCViewModel npcViewModel, VillageTroopsViewModel villageTroopsViewModel, InfoViewModel infoViewModel) : base(SelectedItemStore)
        {
            _villageNavigationStore = villageNavigationStore;
            _contextFactory = contextFactory;

            this.WhenAnyValue(vm => vm._villageNavigationStore.CurrentViewModel)
                .ToProperty(this, vm => vm.CurrentViewModel, out _currentViewModel);

            this.WhenAnyValue(vm => vm.CurrentVillage).BindTo(_selectedItemStore, vm => vm.Village);
            this.WhenAnyValue(vm => vm.CurrentVillage).Where(x => x is not null).Subscribe(x =>
            {
                SetTab(TabType.Normal);
            });
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int accountId)
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
            });
        }

        public void SetTab(TabType tab)
        {
            if (!IsActive) return;
            if (_currentTab == tab) return;
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel.Value;
        }

        public ListBoxItem CurrentVillage
        {
            get => _currentVillage;
            set => this.RaiseAndSetIfChanged(ref _currentVillage, value);
        }
    }
}