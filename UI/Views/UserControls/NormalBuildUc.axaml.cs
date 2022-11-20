using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class NormalBuildUc : ReactiveUserControl<NormalBuildViewModel>
    {
        public NormalBuildUc()
        {
            ViewModel = Locator.Current.GetService<NormalBuildViewModel>();
            InitializeComponent();
            this.WhenActivated((d) =>
            {
                this.BindCommand(ViewModel, vm => vm.BuildCommand, v => v.Build).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ComboBuildings, v => v.Buildings.Items).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Level, v => v.Level.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.MinLevel, v => v.Level.Minimum).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.MaxLevel, v => v.Level.Maximum).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.SelectedBuilding, v => v.Buildings.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedBuildingIndex, v => v.Buildings.SelectedIndex).DisposeWith(d);
            });
        }
    }
}