using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Store
{
    public class VillageNavigationStore : NavigationStore
    {
        public VillageNavigationStore()
        {
            CurrentViewModel = new NoVillageViewModel();
        }
    }
}