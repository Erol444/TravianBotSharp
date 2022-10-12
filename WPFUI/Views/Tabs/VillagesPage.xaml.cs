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
                this.Bind(ViewModel, vm => vm.CurrentIndex, v => v.VillagesGrid.SelectedIndex).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.BuildTab.IsSelected).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageNotSelected, v => v.NoVillageTab.IsSelected).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsVillageNotSelected, v => v.NoVillageTab.Visibility).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.BuildPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVillage, v => v.BuildPage.ViewModel.CurrentVillage).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.BuildTab.Visibility).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.InfoPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVillage, v => v.InfoPage.ViewModel.CurrentVillage).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.InfoTab.Visibility).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.NPCPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVillage, v => v.NPCPage.ViewModel.CurrentVillage).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.NPCTab.Visibility).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CurrentAccount, v => v.SettingsPage.ViewModel.CurrentAccount).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVillage, v => v.SettingsPage.ViewModel.CurrentVillage).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsVillageSelected, v => v.SettingsTab.Visibility).DisposeWith(d);

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}