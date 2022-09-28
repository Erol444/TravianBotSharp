using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for HeroPage.xaml
    /// </summary>
    public partial class HeroPage : ReactivePage<HeroViewModel>
    {
        public HeroPage()
        {
            ViewModel = new();
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

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}