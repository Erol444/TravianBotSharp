using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using TbsCrossPlatform.ViewModels;

namespace TbsCrossPlatform.Views
{
    public partial class LoadingWindow : ReactiveWindow<LoadingViewModel>
    {
        public LoadingWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ViewModel = new();
            Closing += ViewModel.LoadingWindow_Closing;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}