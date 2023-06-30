using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public partial class NoAccountTabBase : ReactiveUserControl<NoAccountViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for NoAccountPage.xaml
    /// </summary>
    public partial class NoAccountTab : NoAccountTabBase
    {
        public NoAccountTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.IsActive, v => v.Visibility).DisposeWith(d);
            });
        }
    }
}