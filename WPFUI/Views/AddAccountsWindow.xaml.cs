using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for AddAccountsWindow.xaml
    /// </summary>
    public partial class AddAccountsWindow : ReactiveWindow<AddAccountsViewModel>
    {
        public AddAccountsWindow()
        {
            ViewModel = new();

            InitializeComponent();
        }
    }
}