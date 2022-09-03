using ReactiveUI;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for AddAccountsPage.xaml
    /// </summary>
    public partial class AddAccountsPage : ReactivePage<AccountsViewModel>
    {
        public AddAccountsPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
            });
        }
    }
}