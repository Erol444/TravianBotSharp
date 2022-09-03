using ReactiveUI;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for EditAccountPage.xaml
    /// </summary>
    public partial class EditAccountPage : ReactivePage<AccountViewModel>
    {
        public EditAccountPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
            });
        }
    }
}