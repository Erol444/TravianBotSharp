using MainCore.Services;
using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.Interfaces;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for OverviewPage.xaml
    /// </summary>
    public partial class OverviewPage : ReactivePage<OverviewViewModel>
    {
        public OverviewPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel,
                    vm => vm.SaveCommand,
                    v => v.SaveButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.ExportCommand,
                    v => v.ExportButton)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.ImportCommand,
                    v => v.ImportButton)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.VillagesSettings,
                    v => v.VillagesGrid.ItemsSource)
                .DisposeWith(d);

                ViewModel.OnActived();
            });
        }
    }
}