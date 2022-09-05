using ReactiveUI;
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
            ViewModel = new();
            InitializeComponent();
            ViewModel.ShowWindow += Show;
            ViewModel.CloseWindow += Hide;

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Text, v => v.Text.Text).DisposeWith(d);
            });
        }
    }
}