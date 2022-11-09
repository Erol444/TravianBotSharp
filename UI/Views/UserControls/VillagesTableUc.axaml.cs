using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class VillagesTableUc : ReactiveUserControl<VillagesTableViewModel>
    {
        public VillagesTableUc()
        {
            ViewModel = Locator.Current.GetService<VillagesTableViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Villages, v => v.Villages.Items).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentVillage, v => v.Villages.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentIndex, v => v.Villages.SelectedIndex).DisposeWith(d);
            });
        }
    }
}