using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.BuildView;

namespace WPFUI.Views.Uc.BuildView
{
    /// <summary>
    /// Interaction logic for NormalBuildUc.xaml
    /// </summary>
    public partial class NormalBuildUc : ReactiveUserControl<NormalBuildViewModel>
    {
        public NormalBuildUc()
        {
            ViewModel = Locator.Current.GetService<NormalBuildViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.BuildCommand, v => v.BuildButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.BuildingBox.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Building, v => v.BuildingBox.SelectedItem).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Level, v => v.LevelText.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Level, v => v.LevelText.Minimum).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsComboBoxEnable, v => v.BuildingBox.IsEnabled).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsLevelEnable, v => v.LevelText.IsEnabled).DisposeWith(d);
            });
        }
    }
}