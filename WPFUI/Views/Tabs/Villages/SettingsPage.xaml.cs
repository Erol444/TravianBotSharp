﻿using ReactiveUI;
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
            AutoNPC.ViewModel = new("Auto NPC when crop is more than", "% of granary");
            AutoNPCWarehouse.ViewModel = new("Auto NPC when any resource is more than", "% of warehouse");
            AutoNPCRatio.ViewModel = new("Ratio");
            TroopUpgrade.ViewModel = new("Troop will be upgraded");
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
                this.Bind(ViewModel, vm => vm.Settings.IsNPCOverflow, v => v.NPCCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCPercent, v => v.AutoNPC.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCWarehousePercent, v => v.AutoNPCWarehouse.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCWood, v => v.AutoNPCRatio.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCClay, v => v.AutoNPCRatio.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCIron, v => v.AutoNPCRatio.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.AutoNPCCrop, v => v.AutoNPCRatio.ViewModel.Crop).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsUpgradeTroop, v => v.TroopUpgradeCheckBox.IsChecked).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TroopUpgrade, v => v.TroopUpgrade.ViewModel.Troops).DisposeWith(d);
            });
        }
    }
}