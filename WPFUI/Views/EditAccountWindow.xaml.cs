using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for EditAccountWindow.xaml
    /// </summary>
    public partial class EditAccountWindow : ReactiveWindow<EditAccountViewModel>
    {
        public EditAccountWindow()
        {
            ViewModel = new();

            InitializeComponent();
        }
    }
}