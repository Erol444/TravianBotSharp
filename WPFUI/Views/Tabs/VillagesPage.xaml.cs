using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for VillagesPage.xaml
    /// </summary>
    public partial class VillagesPage : ReactivePage<VillagesViewModel>
    {
        public VillagesPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Villages, v => v.VillagesGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentVillage, v => v.VillagesGrid.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.BuildTab.IsSelected).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageNotSelected, v => v.NoVillageTab.IsSelected).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsVillageNotSelected, v => v.NoVillageTab.Visibility).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.AccountId, v => v.BuildPage.ViewModel.AccountId).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVillage.Id, v => v.BuildPage.ViewModel.VillageId).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.BuildTab.Visibility).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.AccountId, v => v.InfoPage.ViewModel.AccountId).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVillage.Id, v => v.InfoPage.ViewModel.VillageId).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.InfoTab.Visibility).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.AccountId, v => v.SettingsPage.ViewModel.AccountId).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVillage.Id, v => v.SettingsPage.ViewModel.VillageId).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.SettingsTab.Visibility).DisposeWith(d);

                ViewModel.OnActived();
            });
        }
    }
}