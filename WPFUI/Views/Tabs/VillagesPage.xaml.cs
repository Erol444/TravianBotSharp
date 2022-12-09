using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for VillagesPage.xaml
    /// </summary>
    public partial class VillagesPage : ReactivePage<VillagesViewModel>
    {
        public VillagesPage()
        {
            ViewModel = Locator.Current.GetService<VillagesViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Villages, v => v.VillagesGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentVillage, v => v.VillagesGrid.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentIndex, v => v.VillagesGrid.SelectedIndex).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Tabs, v => v.Tabs.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TabIndex, v => v.Tabs.SelectedIndex).DisposeWith(d);
            });
        }
    }
}