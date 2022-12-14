using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using WPFUI.Views.Tabs.Villages;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : AccountTabBaseViewModel
    {
        public VillagesViewModel()
        {
            this.WhenAnyValue(vm => vm.CurrentVillage).BindTo(_selectorViewModel, vm => vm.Village);
            this.WhenAnyValue(x => x.CurrentIndex).Subscribe(x =>
            {
                if (x == -1) return;
                if (_current == TabType.Normal) return;
                SetTab(TabType.Normal);
            });
            _tabsHolder = new()
            {
                {
                    TabType.NoAccount, new TabItemModel[]
                    {
                        new("No account", new NoVillagePage()) ,
                    }
                },
                {
                    TabType.Normal, new TabItemModel[]
                    {
                        new("Build", new BuildPage()),
                        new("Settings", new SettingsPage()),
                        new("NPC", new NPCPage()),
                        new("Troop", new TroopsPage()),
                        new("Info", new InfoPage()),
                    }
                }
            };
            Tabs = new()
            {
                _tabsHolder[TabType.NoAccount]
            };
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int accountId)
        {
            OldVillage ??= CurrentVillage;
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
                if (villages.Any())
                {
                    Villages.AddRange(villages);
                    var vill = Villages.FirstOrDefault(x => x.Id == OldVillage?.Id);

                    if (vill is not null) CurrentIndex = Villages.IndexOf(vill);
                    else CurrentIndex = 0;
                    OldVillage = null;
                }
            });
        }

        public void SetTab(TabType tab)
        {
            if (!IsActive) return;
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Tabs.Clear();
                Tabs.AddRange(_tabsHolder[tab]);
                TabIndex = 0;
                _current = tab;
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

        public ObservableCollection<TabItemModel> Tabs { get; }
        private readonly Dictionary<TabType, TabItemModel[]> _tabsHolder;
        private TabType _current;

        private int _tabIndex;

        public int TabIndex
        {
            get => _tabIndex;
            set => this.RaiseAndSetIfChanged(ref _tabIndex, value);
        }
    }
}