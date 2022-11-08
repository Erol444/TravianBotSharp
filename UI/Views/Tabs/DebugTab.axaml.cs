using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.Tabs;

namespace UI.Views.Tabs
{
    public partial class DebugTab : ReactiveUserControl<DebugViewModel>
    {
        public DebugTab()
        {
            ViewModel = Locator.Current.GetService<DebugViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.GetHelpCommand, v => v.ReportButton).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Logs, v => v.LogGrid.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Tasks, v => v.TaskGird.Items).DisposeWith(d);
            });
        }
    }
}