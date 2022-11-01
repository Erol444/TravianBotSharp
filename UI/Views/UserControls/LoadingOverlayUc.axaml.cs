using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class LoadingOverlayUc : ReactiveUserControl<LoadingOverlayViewModel>
    {
        public LoadingOverlayUc()
        {
            ViewModel = Locator.Current.GetService<LoadingOverlayViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.IsLoading, v => v.Overlay.IsVisible).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.LoadingText, v => v.Text.Text).DisposeWith(d);
            });
        }
    }
}