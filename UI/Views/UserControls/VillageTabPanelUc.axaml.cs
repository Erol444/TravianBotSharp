using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class VillageTabPanelUc : ReactiveUserControl<VillageTabPanelViewModel>
    {
        public VillageTabPanelUc()
        {
            ViewModel = Locator.Current.GetService<VillageTabPanelViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Tabs, v => v.TabControl.Items).DisposeWith(d);
            });
        }
    }
}