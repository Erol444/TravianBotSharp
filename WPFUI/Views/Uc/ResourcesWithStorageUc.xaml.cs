using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for ResourcesWithStorageUc.xaml
    /// </summary>
    public partial class ResourcesWithStorageUc : ReactiveUserControl<ResourcesWithStorageViewModel>
    {
        public ResourcesWithStorageUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Warehouse, v => v.Warehouse.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Wood, v => v.Wood.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Clay, v => v.Clay.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Iron, v => v.Iron.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Granary, v => v.Granary.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Crop, v => v.Crop.Value).DisposeWith(d);
            });
        }
    }
}