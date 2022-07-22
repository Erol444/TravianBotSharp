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
                this.OneWayBind(ViewModel,
                    vm => vm.Villages,
                    v => v.VillagesGrid.ItemsSource)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.IsVillageNotSelected,
                    v => v.NoVillageTab.Visibility)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.IsVillageSelected,
                    v => v.BuildTab.Visibility)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                   vm => vm.IsVillageSelected,
                   v => v.InfoTab.Visibility)
               .DisposeWith(d);

                ViewModel.LoadData(App.AccountId);
            });
        }
    }
}