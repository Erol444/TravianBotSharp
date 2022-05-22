using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TbsCrossPlatform.Views
{
    public partial class VillageUc : UserControl
    {
        public VillageUc()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
