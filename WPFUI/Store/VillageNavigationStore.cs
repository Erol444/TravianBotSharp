using WPFUI.Models;
using WPFUI.ViewModels.Tabs.Villages;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Store
{
    public class VillageNavigationStore : NavigationStore
    {
        private readonly NoVillageViewModel _noVillageViewModel;
        private readonly BuildViewModel _buildViewModel;
        private readonly VillageSettingsViewModel _villageSettingsViewModel;
        private readonly NPCViewModel _npcViewModel;
        private readonly VillageTroopsViewModel _villageTroopsViewModel;
        private readonly InfoViewModel _infoViewModel;

        public VillageNavigationStore(NoVillageViewModel noVillageViewModel, BuildViewModel buildViewModel, VillageSettingsViewModel villageSettingsViewModel, NPCViewModel npcViewModel, VillageTroopsViewModel villageTroopsViewModel, InfoViewModel infoViewModel)
        {
            _noVillageViewModel = noVillageViewModel;
            _buildViewModel = buildViewModel;
            _villageSettingsViewModel = villageSettingsViewModel;
            _npcViewModel = npcViewModel;
            _villageTroopsViewModel = villageTroopsViewModel;
            _infoViewModel = infoViewModel;
        }

        protected override void InitTabHolder()
        {
            _tabsHolder = new()
            {
                {
                    TabType.NoAccount, new TabHeaderViewModel[]
                    {
                        new("No village", _noVillageViewModel, this) ,
                    }
                },
                {
                    TabType.Normal, new TabHeaderViewModel[]
                    {
                        new("Build",  _buildViewModel, this),
                        new("Settings", _villageSettingsViewModel, this),
                        new("NPC", _npcViewModel, this),
                        new("Troop", _villageTroopsViewModel, this),
                        new("Info", _infoViewModel, this),
                    }
                }
            };
        }
    }
}