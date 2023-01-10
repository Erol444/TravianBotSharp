using ReactiveUI;
using Splat;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    /// <summary>
    /// Interaction logic for BuildPage.xaml
    /// </summary>
    public partial class BuildPage : ReactivePage<BuildViewModel>
    {
        public BuildPage()
        {
            ViewModel = Locator.Current.GetService<BuildViewModel>();
            InitializeComponent();
        }
    }
}