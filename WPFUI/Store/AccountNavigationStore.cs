using WPFUI.Models;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Store
{
    public class AccountNavigationStore : NavigationStore
    {
        private readonly NoAccountViewModel _noAccountViewModel;
        private readonly AddAccountViewModel _addAccountViewModel;
        private readonly AddAccountsViewModel _addAccountsViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly HeroViewModel _heroViewModel;
        private readonly VillagesViewModel _villagesViewModel;
        private readonly FarmingViewModel _farmingViewModel;
        private readonly EditAccountViewModel _editAccountViewModel;
        private readonly DebugViewModel _debugViewModel;

        public AccountNavigationStore(NoAccountViewModel noAccountViewModel, AddAccountViewModel addAccountViewModel, AddAccountsViewModel addAccountsViewModel, SettingsViewModel settingsViewModel, HeroViewModel heroViewModel, VillagesViewModel villagesViewModel, FarmingViewModel farmingViewModel, EditAccountViewModel editAccountViewModel, DebugViewModel debugViewModel)
        {
            _noAccountViewModel = noAccountViewModel;
            _addAccountViewModel = addAccountViewModel;
            _addAccountsViewModel = addAccountsViewModel;
            _settingsViewModel = settingsViewModel;
            _heroViewModel = heroViewModel;
            _villagesViewModel = villagesViewModel;
            _farmingViewModel = farmingViewModel;
            _editAccountViewModel = editAccountViewModel;
            _debugViewModel = debugViewModel;
        }

        protected override void InitTabHolder()
        {
            _tabsHolder = new()
            {
                {
                    TabType.NoAccount, new TabHeaderViewModel[]
                    {
                        new("No account", _noAccountViewModel, this),
                    }
                },
                {
                    TabType.AddAccount, new TabHeaderViewModel[]
                    {
                        new("Add account", _addAccountViewModel, this),
                    }
                },
                {
                    TabType.AddAccounts, new TabHeaderViewModel[]
                    {
                        new("Add accounts", _addAccountsViewModel, this),
                    }
                },
                {
                    TabType.Normal, new TabHeaderViewModel[]
                    {
                        new("Settings", _settingsViewModel, this),
                        new("Hero", _heroViewModel, this),
                        new("Villages", _villagesViewModel, this),
                        new("Farming", _farmingViewModel, this),
                        new("Edit account", _editAccountViewModel, this),
                        new("Debug", _debugViewModel, this),
                    }
                }
            };
        }
    }
}