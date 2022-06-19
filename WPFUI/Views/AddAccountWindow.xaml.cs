using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for AddAccountWindow.xaml
    /// </summary>
    public partial class AddAccountWindow : ReactiveWindow<AddAccountViewModel>
    {
        public AddAccountWindow()
        {
            ViewModel = new();

            InitializeComponent();
        }
    }
}