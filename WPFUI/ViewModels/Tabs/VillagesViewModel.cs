using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs.Villages;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : AccountTabBaseViewModel
    {
        private readonly VillageNavigationStore _villageNavigationStore;

        public ObservableCollection<TabHeaderViewModel> TabHeaders { get; } = new();
        private readonly Dictionary<TabType, TabHeaderViewModel[]> _tabsHolder;
        private TabType _currentTab;

        private readonly ObservableAsPropertyHelper<ViewModelBase> _currentViewModel;

        public ObservableCollection<VillageModel> Villages { get; } = new();

        private VillageModel _currentVillage;

        public VillagesViewModel(VillageNavigationStore villageNavigationStore, NoVillageViewModel noVillageViewModel, BuildViewModel buildViewModel, VillageSettingsViewModel villageSettingsViewModel, NPCViewModel npcViewModel, VillageTroopsViewModel villageTroopsViewModel, InfoViewModel infoViewModel)
        {
            _villageNavigationStore = villageNavigationStore;

            _tabsHolder = new()
            {
                {
                    TabType.NoAccount, new TabHeaderViewModel[]
                    {
                        new("No village", noVillageViewModel, villageNavigationStore) ,
                    }
                },
                {
                    TabType.Normal, new TabHeaderViewModel[]
                    {
                        new("Build",  buildViewModel, villageNavigationStore),
                        new("Settings", villageSettingsViewModel, villageNavigationStore),
                        new("NPC", npcViewModel, villageNavigationStore),
                        new("Troop", villageTroopsViewModel, villageNavigationStore),
                        new("Info", infoViewModel, villageNavigationStore),
                    }
                }
            };
            this.WhenAnyValue(vm => vm._villageNavigationStore.CurrentViewModel)
                .ToProperty(this, vm => vm.CurrentViewModel, out _currentViewModel);
            this.WhenAnyValue(vm => vm.CurrentViewModel)
                .Select(x => x is not null)
                .Subscribe(x => TabChanged());

            this.WhenAnyValue(vm => vm.CurrentVillage).BindTo(_selectorViewModel, vm => vm.Village);
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
            var oldIndex = -1;
            if (CurrentVillage is not null)
            {
                oldIndex = CurrentVillage.Id;
            }

            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages
                .Where(x => x.AccountId == accountId)
                .Select(village => new VillageModel()
                {
                    Id = village.Id,
                    Name = village.Name,
                    Coords = $"{village.X}|{village.Y}",
                })
                .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Villages.Clear();
                Villages.AddRange(villages);
                if (villages.Any())
                {
                    if (oldIndex == -1)
                    {
                        CurrentVillage = Villages.First();
                    }
                    else
                    {
                        var build = Villages.FirstOrDefault(x => x.Id == oldIndex);
                        CurrentVillage = build;
                    }
                }
            });
        }

        public void SetTab(TabType tab)
        {
            if (!IsActive) return;
            if (_currentTab == tab) return;
            _currentTab = tab;
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                TabHeaders.Clear();
                TabHeaders.AddRange(_tabsHolder[tab]);
                _tabsHolder[tab].First().Select(true);
            });
        }

        private void TabChanged()
        {
            foreach (var tab in _tabsHolder[_currentTab])
            {
                tab.IsSelected = false;
            }
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel.Value;
        }

        public VillageModel CurrentVillage
        {
            get => _currentVillage;
            set => this.RaiseAndSetIfChanged(ref _currentVillage, value);
        }
    }
}