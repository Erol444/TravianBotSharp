using ReactiveUI;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    public class WaitingOverlayUcBase : ReactiveUserControl<WaitingOverlayViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for WaitingOverlayUc.xaml
    /// </summary>
    public partial class WaitingOverlayUc : WaitingOverlayUcBase
    {
        public WaitingOverlayUc()
        {
            InitializeComponent();
        }
    }
}