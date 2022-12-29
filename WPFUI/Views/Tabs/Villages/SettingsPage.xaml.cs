using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : ReactivePage<VillageSettingsViewModel>
    {
        public SettingsPage()
        {
            ViewModel = Locator.Current.GetService<VillageSettingsViewModel>();
            InitializeComponent();
            Complete.ViewModel = new("Auto complete upgrade when queue is longer than", "min(s)");
            WatchAds.ViewModel = new("Using ads upgrade button when building time is longer than", "min(s)");
            Refresh.ViewModel = new("Refresh interval", "min(s)");
            AutoNPC.ViewModel = new("Auto NPC when crop is more than", "% of granary (this need auto refresh)");
            AutoNPCWarehouse.ViewModel = new("Auto NPC when any resource is more than", "% of warehouse (this need auto refresh)");
            AutoNPCRatio.ViewModel = new("Ratio");
            TroopUpgrade.ViewModel = new("Troop will be upgraded");
            SendOutLimit.ViewModel = new("Upper Limit");
            SendTo.ViewModel = new("Send resources to");
            SendInLimit.ViewModel = new("Lower Limit");
            SendFrom.ViewModel = new("Get resources from");
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsUseHeroRes, v => v.UseHeroResCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.IsIgnoreRomanAdvantage, v => v.IgnoreRomanAdvantageCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.IsInstantComplete, v => v.Complete.ViewModel.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.InstantCompleteTime, v => v.Complete.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.IsAdsUpgrade, v => v.WatchAds.ViewModel.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AdsUpgradeTime, v => v.WatchAds.ViewModel.Value).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsAutoRefresh, v => v.RefreshCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoRefreshTime, v => v.Refresh.ViewModel.MainValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoRefreshTimeTolerance, v => v.Refresh.ViewModel.ToleranceValue).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsAutoNPC, v => v.AutoNPC.ViewModel.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.IsAutoNPCWarehouse, v => v.AutoNPCWarehouse.ViewModel.IsChecked).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsNPCOverflow, v => v.NPCCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCPercent, v => v.AutoNPC.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCWarehousePercent, v => v.AutoNPCWarehouse.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCWood, v => v.AutoNPCRatio.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCClay, v => v.AutoNPCRatio.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCIron, v => v.AutoNPCRatio.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCCrop, v => v.AutoNPCRatio.ViewModel.Crop).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsUpgradeTroop, v => v.TroopUpgradeCheckBox.IsChecked).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TroopUpgrade, v => v.TroopUpgrade.ViewModel.Troops).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsSendExcessResources, v => v.IsSendExcessResources.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessWood, v => v.SendOutLimit.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessClay, v => v.SendOutLimit.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessIron, v => v.SendOutLimit.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessCrop, v => v.SendOutLimit.ViewModel.Crop).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.SendExcessToX, v => v.SendTo.ViewModel.XCoordinate).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessToY, v => v.SendTo.ViewModel.YCoordinate).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsGetMissingResources, v => v.IsGetMissingResources.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.GetMissingWood, v => v.SendInLimit.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.GetMissingClay, v => v.SendInLimit.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.GetMissingIron, v => v.SendInLimit.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.GetMissingCrop, v => v.SendInLimit.ViewModel.Crop).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.SendFromX, v => v.SendFrom.ViewModel.XCoordinate).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendFromY, v => v.SendFrom.ViewModel.YCoordinate).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.StartResourcesCommand, v => v.StartSendingResources).DisposeWith(d);

            });
        }
    }
}