using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using TbsCrossPlatform.ViewModels;

namespace TbsCrossPlatform.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ViewModel = new();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}