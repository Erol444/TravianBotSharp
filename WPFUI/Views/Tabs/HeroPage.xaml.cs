using ReactiveUI;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for HeroPage.xaml
    /// </summary>
    public partial class HeroPage : ReactivePage<HeroViewModel>
    {
        public HeroPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                ViewModel.OnActived();
            });
        }
    }
}