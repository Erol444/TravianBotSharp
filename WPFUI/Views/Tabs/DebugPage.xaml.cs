using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public partial class DebugPageBase : ReactivePage<DebugViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for DebugPage.xaml
    /// </summary>
    public partial class DebugPage : DebugPageBase
    {
        public DebugPage()
        {
            ViewModel = Locator.Current.GetService<DebugViewModel>();
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Logs, v => v.LogGrid.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Tasks, v => v.TaskGird.ItemsSource).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.GetHelpCommand, v => v.ReportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LogFolderCommand, v => v.LogButton).DisposeWith(d);
            });
        }
    }
}