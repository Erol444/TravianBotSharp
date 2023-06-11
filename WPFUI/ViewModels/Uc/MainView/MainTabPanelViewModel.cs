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
using WPFUI.ViewModels.Tabs;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class MainTabPanelViewModel : ActivatableViewModelBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly ObservableAsPropertyHelper<ViewModelBase> _currentViewModel;

        public ObservableCollection<TabHeaderViewModel> TabHeaders { get; } = new();
        private readonly Dictionary<TabType, TabHeaderViewModel[]> _tabsHolder;
        private TabType _currentTab;

        public MainTabPanelViewModel(NavigationStore navigationStore, NoAccountViewModel noAccountViewModel, AddAccountViewModel addAccountViewModel, AddAccountsViewModel addAccountsViewModel, SettingsViewModel settingsViewModel, HeroViewModel heroViewModel, VillagesViewModel villagesViewModel, FarmingViewModel farmingViewModel, EditAccountViewModel editAccountViewModel, DebugViewModel debugViewModel)
        {
            _navigationStore = navigationStore;
            _tabsHolder = new()
            {
                {
                    TabType.NoAccount, new TabHeaderViewModel[]
                    {
                        new("No account", noAccountViewModel,navigationStore    ) ,
                    }
                },
                {
                    TabType.AddAccount, new TabHeaderViewModel[]
                    {
                        new("Add account", addAccountViewModel, navigationStore) ,
                    }
                },
                {
                    TabType.AddAccounts, new TabHeaderViewModel[]
                    {
                        new("Add accounts", addAccountsViewModel,navigationStore) ,
                    }
                },
                {
                    TabType.Normal, new TabHeaderViewModel[]
                    {
                        new("Settings", settingsViewModel   , navigationStore),
                        new("Hero", heroViewModel, navigationStore),
                        new("Villages", villagesViewModel, navigationStore),
                        new("Farming", farmingViewModel, navigationStore),
                        new("Edit account", editAccountViewModel, navigationStore),
                        new("Debug", debugViewModel, navigationStore),
                    }
                }
            };

            this.WhenAnyValue(vm => vm._navigationStore.CurrentViewModel)
                .ToProperty(this, vm => vm.CurrentViewModel, out _currentViewModel);
            this.WhenAnyValue(vm => vm.CurrentViewModel)
                .Select(x => x is not null)
                .Subscribe(x => TabChanged());
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
    }
}