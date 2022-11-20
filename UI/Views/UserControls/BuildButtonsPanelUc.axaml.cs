using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class BuildButtonsPanelUc : ReactiveUserControl<BuildButtonPanelViewModel>
    {
        public BuildButtonsPanelUc()
        {
            ViewModel = Locator.Current.GetService<BuildButtonPanelViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.TopCommand, v => v.TopButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.BottomCommand, v => v.BottomButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.UpCommand, v => v.UpButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DownCommand, v => v.DownButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAllCommand, v => v.DeleteAllButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
            });
        }
    }
}