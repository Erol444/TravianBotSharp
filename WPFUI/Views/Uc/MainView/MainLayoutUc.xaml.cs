using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Views.Uc.MainView
{
    public class MainLayoutUcBase : ReactiveUserControl<MainLayoutViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MainLayoutUc.xaml
    /// </summary>
    public partial class MainLayoutUc : MainLayoutUcBase
    {
        public MainLayoutUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.MainTabPanelViewModel, v => v.MainTabPanel.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.MainButtonPanelViewModel, v => v.MainButtonPanel.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AccountListViewModel, v => v.AccountList.ViewModel).DisposeWith(d);
            });
        }
    }
}