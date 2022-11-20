using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.Tabs.Villages;

namespace UI.Views.Tabs.Villages
{
    public partial class BuildTab : ReactiveUserControl<BuildViewModel>
    {
        public BuildTab()
        {
            ViewModel = Locator.Current.GetService<BuildViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.Buildings.Items).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.QueueBuildings, v => v.Queue.Items).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentlyBuildings, v => v.Current.Items).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.CurrentIndexBuilding, v => v.Buildings.SelectedIndex).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentIndexQueue, v => v.Queue.SelectedIndex).DisposeWith(d);
            });
        }
    }
}