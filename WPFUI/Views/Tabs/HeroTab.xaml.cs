using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class HeroTabBase : ReactiveUserControl<HeroViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for HeroPage.xaml
    /// </summary>
    public partial class HeroTab : HeroTabBase
    {
        public HeroTab()
        {
            ViewModel = Locator.Current.GetService<HeroViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.AdventuresCommand, v => v.AdventuresButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.InventoryCommand, v => v.InventoryButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Adventures, v => v.AdventuresGrid.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Inventory, v => v.ItemsGrid.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Equipt, v => v.EquiptGrid.ItemsSource).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Health, v => v.HealthTextbox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Status, v => v.StatusTextbox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AdventureNum, v => v.NumAdventuresTextbox.Text).DisposeWith(d);
            });
        }
    }
}