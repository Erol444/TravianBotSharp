using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for ToleranceUc.xaml
    /// </summary>
    public partial class ToleranceUc : ReactiveUserControl<ToleranceViewModel>
    {
        public ToleranceUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Text, v => v.TextLabel.Content).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Unit, v => v.UnitLabel.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.MainValue, v => v.MainValue.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ToleranceValue, v => v.Tolerance.Text).DisposeWith(d);
            });
        }
    }
}