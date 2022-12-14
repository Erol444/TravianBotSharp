using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.BuildView;

namespace WPFUI.Views.Uc.BuildView
{
    /// <summary>
    /// Interaction logic for CurrentBuildingListUc.xaml
    /// </summary>
    public partial class CurrentBuildingListUc : ReactiveUserControl<CurrentBuildingListViewModel>
    {
        public CurrentBuildingListUc()
        {
            ViewModel = Locator.Current.GetService<CurrentBuildingListViewModel>();
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.Buildings.ItemsSource).DisposeWith(d);
            });
        }
    }
}