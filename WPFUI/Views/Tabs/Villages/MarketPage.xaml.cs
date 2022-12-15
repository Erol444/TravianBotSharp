using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    /// <summary>
    /// Interaction logic for MarketPage.xaml
    /// </summary>
    public partial class MarketPage : ReactivePage<MarketViewModel>
    {
        public MarketPage()
        {
            ViewModel = Locator.Current.GetService<MarketViewModel>();
            InitializeComponent();

            SendOutLimit.ViewModel = new("Upper Limit:");
            SendTo.ViewModel = new("Send resources to:");
            SendInLimit.ViewModel = new("Lower Limit:");
            SendFrom.ViewModel = new("Get resources from:");
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsSendExcessResources, v => v.IsSendExcessResources.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessWood, v => v.SendOutLimit.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessClay, v => v.SendOutLimit.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessIron, v => v.SendOutLimit.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessCrop, v => v.SendOutLimit.ViewModel.Crop).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.SendExcessToX, v => v.SendTo.ViewModel.XCoordinate).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessToY, v => v.SendTo.ViewModel.YCoordinate).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsGetMissingResources, v => v.IsGetMissingResources.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.GetMissingWood, v => v.SendInLimit.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.GetMissingClay, v => v.SendInLimit.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.GetMissingIron, v => v.SendInLimit.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.GetMissingCrop, v => v.SendInLimit.ViewModel.Crop).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.SendFromX, v => v.SendFrom.ViewModel.XCoordinate).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendFromY, v => v.SendFrom.ViewModel.YCoordinate).DisposeWith(d);
            });
        }
    }
}