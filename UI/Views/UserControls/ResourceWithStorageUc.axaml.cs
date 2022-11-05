using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class ResourceWithStorageUc : ReactiveUserControl<ResourceWithStorageViewModel>
    {
        public ResourceWithStorageUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Text, v => v.Text.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Warehouse, v => v.Warehouse.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Wood, v => v.Wood.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Clay, v => v.Clay.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Iron, v => v.Iron.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Granary, v => v.Granary.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Crop, v => v.Crop.Value).DisposeWith(d);
            });
        }
    }
}