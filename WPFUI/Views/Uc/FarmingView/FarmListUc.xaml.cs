using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.FarmingView;

namespace WPFUI.Views.Uc.FarmingView
{
    /// <summary>
    /// Interaction logic for FarmListUc.xaml
    /// </summary>
    public partial class FarmListUc : ReactiveUserControl<FarmListViewModel>
    {
        public FarmListUc()
        {
            ViewModel = Locator.Current.GetService<FarmListViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.RefreshFarmListsButton).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.FarmList, v => v.FarmListViewer.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentFarm, v => v.FarmListViewer.SelectedItem).DisposeWith(d);
            });
        }
    }
}