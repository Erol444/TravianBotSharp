using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class ResourceUc : ReactiveUserControl<ResourceViewModel>
    {
        public ResourceUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Text, v => v.Text.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Wood, v => v.Wood.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Clay, v => v.Clay.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Iron, v => v.Iron.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Crop, v => v.Crop.Text).DisposeWith(d);
            });
        }
    }
}