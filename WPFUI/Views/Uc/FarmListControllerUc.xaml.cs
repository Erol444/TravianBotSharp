using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for FarmListController.xaml
    /// </summary>
    public partial class FarmListControllerUc : ReactiveUserControl<FarmListControllerViewModel>
    {
        public FarmListControllerUc()
        {
            ViewModel = new();
            InitializeComponent();
            Interval.ViewModel = new("Time for next send", "sec(s)");
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.FarmSetting.IsActive, v => v.ActiveCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.FarmSetting.IntervalTime, v => v.Interval.ViewModel.MainValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.FarmSetting.IntervalDiffTime, v => v.Interval.ViewModel.ToleranceValue).DisposeWith(d);
            });
        }
    }
}