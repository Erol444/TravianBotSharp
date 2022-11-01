using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class TabPanelUc : ReactiveUserControl<TabPanelViewModel>
    {
        public TabPanelUc()
        {
            ViewModel = Locator.Current.GetService<TabPanelViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Tabs, v => v.TabControl.Items).DisposeWith(d);
            });
        }
    }
}