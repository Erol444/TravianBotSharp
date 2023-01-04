using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.FarmingView;

namespace WPFUI.Views.Uc.FarmingView
{
    /// <summary>
    /// Interaction logic for FarmContentUc.xaml
    /// </summary>
    public partial class FarmContentUc : ReactiveUserControl<FarmContentViewModel>
    {
        public FarmContentUc()
        {
            ViewModel = Locator.Current.GetService<FarmContentViewModel>();
            InitializeComponent();
            Interval.ViewModel = new("Time for next send", "sec(s) (this apply to all farm)");
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.IsEnable, v => v.MainGrid.IsEnabled).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.FarmSetting.IsActive, v => v.ActiveCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Interval, v => v.Interval.ViewModel.MainValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.DiffInterval, v => v.Interval.ViewModel.ToleranceValue).DisposeWith(d);
            });
        }
    }
}