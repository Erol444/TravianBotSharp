using Avalonia.ReactiveUI;
using Splat;
using UI.ViewModels.Tabs;

namespace UI.Views.Tabs
{
    public partial class VillagesTab : ReactiveUserControl<VillagesViewModel>
    {
        public VillagesTab()
        {
            ViewModel = Locator.Current.GetService<VillagesViewModel>();
            InitializeComponent();
        }
    }
}