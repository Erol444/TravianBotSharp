using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class VillagesTabBase : ReactiveUserControl<VillagesViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for VillagesPage.xaml
    /// </summary>
    public partial class VillagesTab : VillagesTabBase
    {
        public VillagesTab()
        {
            ViewModel = Locator.Current.GetService<VillagesViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Villages, v => v.VillagesGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentVillage, v => v.VillagesGrid.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.TabHeaders, v => v.Headers.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentViewModel, v => v.TabPanel.Content).DisposeWith(d);
            });
        }
    }
}