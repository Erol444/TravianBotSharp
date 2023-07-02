using ReactiveUI;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class NoVillageTabBase : ReactiveUserControl<NoVillageViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for NoVillagePage.xaml
    /// </summary>
    public partial class NoVillageTab : NoVillageTabBase
    {
        public NoVillageTab()
        {
            InitializeComponent();
        }
    }
}