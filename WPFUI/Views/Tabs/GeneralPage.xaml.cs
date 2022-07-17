using ReactiveUI;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for GeneralPage.xaml
    /// </summary>
    public partial class GeneralPage : ReactivePage<GeneralViewModel>
    {
        public GeneralPage()
        {
            ViewModel = new();
            InitializeComponent();
        }
    }
}