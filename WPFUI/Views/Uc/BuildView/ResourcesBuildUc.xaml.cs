using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.BuildView;

namespace WPFUI.Views.Uc.BuildView
{
    /// <summary>
    /// Interaction logic for ResourcesBuildUc.xaml
    /// </summary>
    public partial class ResourcesBuildUc : ReactiveUserControl<ResourcesBuildViewModel>
    {
        public ResourcesBuildUc()
        {
            ViewModel = Locator.Current.GetService<ResourcesBuildViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.BuildCommand, v => v.BuildButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ComboResTypes, v => v.Type.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ComboStrategy, v => v.Strategy.ItemsSource).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.SelectedResType, v => v.Type.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedBuildingStrategy, v => v.Strategy.SelectedItem).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Level, v => v.LevelText.Value).DisposeWith(d);
            });
        }
    }
}