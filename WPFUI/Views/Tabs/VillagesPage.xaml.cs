using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class VillagesPageBase : ReactivePage<VillagesViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for VillagesPage.xaml
    /// </summary>
    public partial class VillagesPage : VillagesPageBase
    {
        public VillagesPage()
        {
            ViewModel = Locator.Current.GetService<VillagesViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Villages, v => v.VillagesGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentVillage, v => v.VillagesGrid.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Tabs, v => v.Tabs.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TabIndex, v => v.Tabs.SelectedIndex).DisposeWith(d);
            });
        }
    }
}