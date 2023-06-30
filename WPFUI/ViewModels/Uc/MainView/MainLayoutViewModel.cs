using Splat;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class MainLayoutViewModel : ViewModelBase
    {
        private readonly AccountNavigationStore _accountNavigationStore;
        private readonly VillageNavigationStore _villageNavigationStore;

        private readonly MainTabPanelViewModel _mainTabPanelViewModel;
        private readonly MainButtonPanelViewModel _mainButtonPanelViewModel;
        private readonly AccountListViewModel _accountListViewModel;
        public MainTabPanelViewModel MainTabPanelViewModel => _mainTabPanelViewModel;
        public MainButtonPanelViewModel MainButtonPanelViewModel => _mainButtonPanelViewModel;
        public AccountListViewModel AccountListViewModel => _accountListViewModel;

        public MainLayoutViewModel()
        {
            _accountListViewModel = Locator.Current.GetService<AccountListViewModel>();
            _mainButtonPanelViewModel = Locator.Current.GetService<MainButtonPanelViewModel>();
            _mainTabPanelViewModel = Locator.Current.GetService<MainTabPanelViewModel>();

            _accountNavigationStore = Locator.Current.GetService<AccountNavigationStore>();
            _villageNavigationStore = Locator.Current.GetService<VillageNavigationStore>();
        }

        public void Init()
        {
            _accountNavigationStore.Init();
            _villageNavigationStore.Init();

            _mainTabPanelViewModel.IsActive = true;
            _mainButtonPanelViewModel.IsActive = true;
            _accountListViewModel.IsActive = true;
        }
    }
}