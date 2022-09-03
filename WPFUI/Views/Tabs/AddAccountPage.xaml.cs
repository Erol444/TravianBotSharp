using ReactiveUI;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for AddAccountPage.xaml
    /// </summary>
    public partial class AddAccountPage : ReactivePage<AccountViewModel>
    {
        public AddAccountPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
            });
        }
    }
}