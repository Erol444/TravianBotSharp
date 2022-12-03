using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for WaitingWindow.xaml
    /// </summary>
    public partial class WaitingWindow : ReactiveWindow<WaitingViewModel>
    {
        public WaitingWindow()
        {
            ViewModel = Locator.Current.GetService<WaitingViewModel>();
            InitializeComponent();
            ViewModel.ShowWindow = Show;
            ViewModel.CloseWindow = Hide;

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Text, v => v.Text.Text).DisposeWith(d);
            });
        }
    }
}