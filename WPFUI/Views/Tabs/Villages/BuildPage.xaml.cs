using MainCore.Services;
using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.Interfaces;
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
                this.BindCommand(ViewModel,
                    vm => vm.BuildCommand,
                    v => v.BuildButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.TopCommand,
                    v => v.TopButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.BottomCommand,
                    v => v.BottomButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.UpCommand,
                    v => v.UpButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.DownCommand,
                    v => v.DownButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.DeleteCommand,
                    v => v.DeleteButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.DeleteAllCommand,
                    v => v.DeleteAllButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.ImportCommand,
                    v => v.ImportButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.ExportCommand,
                    v => v.ExportButton)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.Buildings,
                    v => v.BuildingsGrid.ItemsSource)
                .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.CurrentBuilding,
                    v => v.BuildingsGrid.SelectedItem)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.CurrentlyBuildings,
                    v => v.CurrentGrid.ItemsSource)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.QueueBuildings,
                    v => v.QueueGrid.ItemsSource)
                .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.CurrentQueueBuilding,
                    v => v.QueueGrid.SelectedItem)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.ComboBuildings,
                    v => v.BuildingsComboBox.ItemsSource)
                .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.SelectedBuilding,
                    v => v.BuildingsComboBox.SelectedItem)
                .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.SelectedBuildingIndex,
                    v => v.BuildingsComboBox.SelectedIndex)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.IsComboActive,
                    v => v.BuildingsComboBox.IsEnabled)
                .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.Level,
                    v => v.LevelTextBox.Text)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.IsLevelActive,
                    v => v.LevelTextBox.IsEnabled)
                .DisposeWith(d);

                ViewModel.OnActived();
            });
        }
    }
}