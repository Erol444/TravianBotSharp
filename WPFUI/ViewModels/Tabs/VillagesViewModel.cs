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
            this.WhenAnyValue(vm => vm.CurrentVillage).Where(x => x is not null).Subscribe(x =>
            {
                if (_current == TabType.Normal) return;
                SetTab(TabType.Normal);
            });
            _tabsHolder = new()
            {
                {
                    TabType.NoAccount, new TabItemModel[]
                    {
                        new("No village", new NoVillagePage()) ,
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