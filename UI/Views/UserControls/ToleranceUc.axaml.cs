using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class ToleranceUc : ReactiveUserControl<ToleranceViewModel>
    {
        public ToleranceUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Text, v => v.Text.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Unit, v => v.Unit.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Value, v => v.Value.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Min, v => v.Value.Minimum).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Tolerance, v => v.Tolerance.Value).DisposeWith(d);
            });
        }
    }
}