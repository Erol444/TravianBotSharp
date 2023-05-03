using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class BuildPageBase : ReactivePage<BuildViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for BuildPage.xaml
    /// </summary>

    public partial class BuildPage : BuildPageBase
    {
        public BuildPage()
        {
            ViewModel = Locator.Current.GetService<BuildViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.Buildings.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentBuilding, v => v.Buildings.SelectedItem).DisposeWith(d);
            });
        }
    }
}