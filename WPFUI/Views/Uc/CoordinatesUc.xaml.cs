using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for CoordinatesUc.xaml
    /// </summary>
    public partial class CoordinatesUc : ReactiveUserControl<CoordinatesViewModel>
    {
        public CoordinatesUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Text, v => v.TextLabel.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.XCoordinate, v => v.XCoordinate.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.YCoordinate, v => v.YCoordinate.Text).DisposeWith(d);
            });
        }
    }
}