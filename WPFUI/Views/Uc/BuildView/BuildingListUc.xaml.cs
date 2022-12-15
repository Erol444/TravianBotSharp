using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.BuildView;

namespace WPFUI.Views.Uc.BuildView
{
    /// <summary>
    /// Interaction logic for BuildingListUc.xaml
    /// </summary>
    public partial class BuildingListUc : ReactiveUserControl<BuildingListViewModel>
    {
        public BuildingListUc()
        {
            ViewModel = Locator.Current.GetService<BuildingListViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.Buildings.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentBuilding, v => v.Buildings.SelectedItem).DisposeWith(d);
            });
        }
    }
}