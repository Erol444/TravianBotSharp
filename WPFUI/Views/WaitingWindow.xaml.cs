using ReactiveUI;
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

            this.OneWayBind(ViewModel,
                vm => vm.Text,
                v => v.Text.Text);
        }
    }
}