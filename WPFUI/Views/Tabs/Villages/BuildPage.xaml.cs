using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    /// <summary>
    /// Interaction logic for BuildPage.xaml
    /// </summary>
    public partial class BuildPage : ReactivePage<BuildViewModel>
    {
        public BuildPage()
        {
            ViewModel = new();
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.TopCommand, v => v.TopButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.BottomCommand, v => v.BottomButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.UpCommand, v => v.UpButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DownCommand, v => v.DownButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAllCommand, v => v.DeleteAllButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.BuildingsGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentBuilding, v => v.BuildingsGrid.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentIndexBuilding, v => v.BuildingsGrid.SelectedIndex).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentlyBuildings, v => v.CurrentGrid.ItemsSource).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.CurrentQueueBuilding, v => v.QueueGrid.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentIndexQueue, v => v.QueueGrid.SelectedIndex).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.QueueBuildings, v => v.QueueGrid.ItemsSource).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.NormalBuildCommand, v => v.NormalBuild.BuildButton).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ComboBuildings, v => v.NormalBuild.BuildingBox.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedBuilding, v => v.NormalBuild.BuildingBox.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedBuildingIndex, v => v.NormalBuild.BuildingBox.SelectedIndex).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsComboActive, v => v.NormalBuild.BuildingBox.IsEnabled).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.NormalLevel, v => v.NormalBuild.LevelText.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsLevelActive, v => v.NormalBuild.LevelText.IsEnabled).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ResBuildCommand, v => v.ResBuild.BuildButton).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedResType, v => v.ResBuild.Type.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedBuildingStrategy, v => v.ResBuild.Strategy.SelectedItem).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ComboResTypes, v => v.ResBuild.Type.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ResLevel, v => v.ResBuild.LevelText.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ComboStrategy, v => v.ResBuild.Strategy.ItemsSource).DisposeWith(d);

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}