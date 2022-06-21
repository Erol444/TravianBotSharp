using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for AddAccountsWindow.xaml
    /// </summary>
    public partial class AccountsWindow : ReactiveWindow<AccountsViewModel>
    {
        public AccountsWindow()
        {
            ViewModel = new();

            InitializeComponent();
        }
    }
}