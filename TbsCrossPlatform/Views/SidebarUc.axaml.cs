using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using TbsCrossPlatform.ViewModels;

namespace TbsCrossPlatform.Views
{
    public partial class SidebarUc : ReactiveUserControl<SidebarViewModel>
    {
        public SidebarUc()
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