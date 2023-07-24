using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class VillagesTabBase : ReactiveUserControl<VillagesViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for VillagesPage.xaml
    /// </summary>
    public partial class VillagesTab : VillagesTabBase
    {
        public VillagesTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Villages, v => v.VillagesGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentVillage, v => v.VillagesGrid.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.NoVillageViewModel, v => v.NoVillage.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.BuildViewModel, v => v.Build.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VillageSettingsViewModel, v => v.VillageSettings.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.NPCViewModel, v => v.NPC.ViewModel).DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.VillageTroopsViewModel, v => v.Troops.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.InfoViewModel, v => v.Info.ViewModel).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.IsNoVillageTabVisible, v => v.NoVillageTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.IsNormalTabVisible, v => v.BuildTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.IsNormalTabVisible, v => v.VillageSettingsTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.IsNormalTabVisible, v => v.NPCTab.Visibility).DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.VillageTabStore.IsNormalTabVisible, v => v.TroopsTab.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VillageTabStore.IsNormalTabVisible, v => v.InfoTab.Visibility).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.NoVillageViewModel.IsActive, v => v.NoVillageTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.BuildViewModel.IsActive, v => v.BuildTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.VillageSettingsViewModel.IsActive, v => v.VillageSettingsTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.NPCViewModel.IsActive, v => v.NPCTab.IsSelected).DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.VillageTroopsViewModel.IsActive, v => v.TroopsTab.IsSelected).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.InfoViewModel.IsActive, v => v.InfoTab.IsSelected).DisposeWith(d);
            });
        }
    }
}