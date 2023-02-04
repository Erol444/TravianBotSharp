using ReactiveUI;
using Splat;
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
            ViewModel = Locator.Current.GetService<FarmingViewModel>();
            InitializeComponent();
            Interval.ViewModel = new("Time for next send", "sec(s)");
            this.WhenActivated(d =>
            {
                //command
                this.BindCommand(ViewModel, vm => vm.StartCommand, v => v.StartButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.StopCommand, v => v.StopButton).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ActiveCommand, v => v.ActiveButton).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.RefreshFarmListsButton).DisposeWith(d);

                // farm list
                this.OneWayBind(ViewModel, vm => vm.FarmList, v => v.FarmListViewer.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentFarm, v => v.FarmListViewer.SelectedItem).DisposeWith(d);

                // interval
                this.Bind(ViewModel, vm => vm.Interval, v => v.Interval.ViewModel.MainValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.DiffInterval, v => v.Interval.ViewModel.ToleranceValue).DisposeWith(d);

                // active content button
                this.Bind(ViewModel, vm => vm.ContentButton, v => v.ActiveButton.Content).DisposeWith(d);
            });
        }
    }
}