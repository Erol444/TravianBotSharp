using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Views.Uc.MainView
{
    public class MainTabPanelUcBase : ReactiveUserControl<MainTabPanelViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MainTabPanelUc.xaml
    /// </summary>
    public partial class MainTabPanelUc : MainTabPanelUcBase
    {
        public MainTabPanelUc()
        {
            ViewModel = Locator.Current.GetService<MainTabPanelViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.TabHeaders, v => v.Headers.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentViewModel, v => v.TabPanel.Content).DisposeWith(d);
            });
        }
    }
}