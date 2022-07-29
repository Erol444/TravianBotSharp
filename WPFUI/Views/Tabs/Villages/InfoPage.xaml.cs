using MainCore.Services;
using ReactiveUI;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    /// <summary>
    /// Interaction logic for InfoPage.xaml
    /// </summary>
    public partial class InfoPage : ReactivePage<InfoViewModel>, IVillageTabPage
    {
        public int VillageId { get; set; }

        public InfoPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                App.GetService<IEventManager>().OnTabActived(ViewModel.GetType(), VillageId);
            });
        }
    }
}