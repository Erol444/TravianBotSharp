﻿using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class VillageSettingsTabBase : ReactiveUserControl<VillageSettingsViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class VillageSettingsTab : VillageSettingsTabBase
    {
        public VillageSettingsTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.UseHeroRes, v => v.UseHeroResCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IgnoreRoman, v => v.IgnoreRomanAdvantageCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AutoComplete, v => v.Complete.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.WatchAds, v => v.WatchAds.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsAutoRefresh, v => v.RefreshCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AutoRefresh, v => v.Refresh.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.AutoNPCCrop, v => v.AutoNPC.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AutoNPCResource, v => v.AutoNPCWarehouse.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsAutoNPCOverflow, v => v.NPCCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.RatioNPC, v => v.AutoNPCRatio.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.TimeTrain, v => v.TimeTrain.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsMaxTrain, v => v.IsMaxTrain.IsChecked).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.BarrackTraining, v => v.BarrackTrain.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.StableTraining, v => v.StableTrain.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.WorkshopTraining, v => v.WorkshopTrain.ViewModel).DisposeWith(d);
            });
        }
    }
}