using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using TbsCrossPlatform.ViewModels;

namespace TbsCrossPlatform.Views
{
    public partial class EditAccountWindow : ReactiveWindow<EditAccountViewModel>

    {
        public EditAccountWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ViewModel = new();
            Opened += ViewModel.Opened;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}