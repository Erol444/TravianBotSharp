using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class BuildTabBase : ReactiveUserControl<BuildViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for BuildPage.xaml
    /// </summary>

    public partial class BuildTab : BuildTabBase
    {
        public BuildTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.CurrentlyBuildings, v => v.CurrentlyBuildings.ItemsSource).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.Buildings.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentBuilding, v => v.Buildings.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.NormalBuildings, v => v.NormalBuildings.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentNormalBuilding, v => v.NormalBuildings.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsComboBoxEnable, v => v.NormalBuildings.IsEnabled).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.NormalLevel, v => v.NormalLevel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsLevelEnable, v => v.NormalLevel.IsEnabled).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.NormalBuildCommand, v => v.NormalBuild).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ComboResTypes, v => v.ResType.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentResType, v => v.NormalBuildings.SelectedItem).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ComboStrategy, v => v.Strategy.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentStrategy, v => v.NormalBuildings.SelectedItem).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.ResourceLevel, v => v.ResourceLevel.Value).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.ResourceBuildCommand, v => v.ResourceBuild).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.TopCommand, v => v.TopButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.BottomCommand, v => v.BottomButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.UpCommand, v => v.UpButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DownCommand, v => v.DownButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAllCommand, v => v.DeleteAllButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.QueueBuildings, v => v.QueueBuildings.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentQueueBuilding, v => v.QueueBuildings.SelectedItem).DisposeWith(d);
            });
        }
    }
}