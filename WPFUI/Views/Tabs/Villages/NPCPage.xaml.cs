using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class NPCTabBase : ReactiveUserControl<NPCViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for NPCPage.xaml
    /// </summary>
    public partial class NPCTab : NPCTabBase
    {
        public NPCTab()
        {
            ViewModel = Locator.Current.GetService<NPCViewModel>();
            InitializeComponent();

            Storage.ViewModel = new();
            Ratio.ViewModel = new();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.UpdateResource).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.NPCCommand, v => v.NPC).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.LastUpdate, v => v.UpdateTime.Text).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Resources.Warehouse, v => v.Storage.ViewModel.Warehouse).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Wood, v => v.Storage.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Clay, v => v.Storage.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Iron, v => v.Storage.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Granary, v => v.Storage.ViewModel.Granary).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Resources.Crop, v => v.Storage.ViewModel.Crop).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Ratio.Wood, v => v.Ratio.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Ratio.Clay, v => v.Ratio.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Ratio.Iron, v => v.Ratio.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Ratio.Crop, v => v.Ratio.ViewModel.Crop).DisposeWith(d);
            });
        }
    }
}