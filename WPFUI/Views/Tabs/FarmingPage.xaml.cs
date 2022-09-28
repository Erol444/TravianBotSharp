using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for FarmingPage.xaml
    /// </summary>
    public partial class FarmingPage : ReactivePage<FarmingViewModel>
    {
        public FarmingPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.RefreshFarmListsButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.StartCommand, v => v.StartButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.StopCommand, v => v.StopButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.FarmList, v => v.FarmListViewer.ItemsSource).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.CurrentFarm, v => v.FarmListViewer.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CurrentFarm, v => v.FarmListController.ViewModel.CurrentFarm).DisposeWith(d);

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}