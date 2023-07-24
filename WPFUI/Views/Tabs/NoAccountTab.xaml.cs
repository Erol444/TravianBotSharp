using ReactiveUI;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class NoAccountTabBase : ReactiveUserControl<NoAccountViewModel>
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
        }
    }
}