using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Views.Uc.MainView
{
    /// <summary>
    /// Interaction logic for MainTabPanelUc.xaml
    /// </summary>
    public partial class MainTabPanelUc : ReactiveUserControl<MainTabPanelViewModel>
    {
        public MainTabPanelUc()
        {
            ViewModel = Locator.Current.GetService<MainTabPanelViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Tabs, v => v.Tabs.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TabIndex, v => v.Tabs.SelectedIndex).DisposeWith(d);
            });
        }
    }
}