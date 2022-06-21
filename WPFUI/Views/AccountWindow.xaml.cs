using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for EditAccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : ReactiveWindow<AccountViewModel>
    {
        public AccountWindow()
        {
            ViewModel = new();

            InitializeComponent();
        }
    }
}