using DynamicData;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using WPFUI.Views.Tabs;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class MainTabPanelViewModel : ActivatableViewModelBase
    {
        private readonly SelectorViewModel _selectorViewModel;

        public MainTabPanelViewModel()
        {
            _selectorViewModel = Locator.Current.GetService<SelectorViewModel>();
            this.WhenAnyValue(vm => vm._selectorViewModel.IsAccountSelected)
                .Where(x => x)
                .Subscribe(_ => SetTab(TabType.Normal));
            _tabsHolder = new()
            {
                {
                    TabType.NoAccount, new TabItemModel[]
                    {
                        new("No account", new NoAccountPage()) ,
                    }
                },
                {
                    TabType.AddAccount, new TabItemModel[]
                    {
                        new("Add account", new AddAccountPage()),
                    }
                },
                {
                    TabType.AddAccounts, new TabItemModel[]
                    {
                        new("Add accounts", new AddAccountsPage()),
                    }
                },
                {
                    TabType.Normal, new TabItemModel[]
                    {
                        new("General", new GeneralPage()),
                        new("Settings", new SettingsPage()),
                        new("Hero", new HeroPage()),
                        new("Villages", new VillagesPage()),
                        new("Farming", new FarmingPage()),
                        new("Debug", new DebugPage()),
                        new("Edit account", new EditAccountPage()),
                    }
                }
            };
            Tabs = new()
            {
                _tabsHolder[TabType.NoAccount][0],
            };
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

        public ObservableCollection<TabItemModel> Tabs { get; }

        private readonly Dictionary<TabType, TabItemModel[]> _tabsHolder;
        private TabType _current;
        public TabType Current => _current;

        private int _tabIndex;

        public int TabIndex
        {
            get => _tabIndex;
            set => this.RaiseAndSetIfChanged(ref _tabIndex, value);
        }
    }
}