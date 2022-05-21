using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using TbsCrossPlatform.ViewModels;

namespace TbsCrossPlatform.Views
{
    public partial class SidebarVillageUc : ReactiveUserControl<SidebarVillageViewModel>
    {
        public SidebarVillageUc()
        {
            InitializeComponent();
            ViewModel = new();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}