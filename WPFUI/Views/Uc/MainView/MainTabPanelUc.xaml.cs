using ReactiveUI;
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
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.TabHeaders, v => v.Headers.ItemsSource).DisposeWith(d);
            });
        }
    }
}