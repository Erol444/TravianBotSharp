using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;
using TbsCrossPlatform.Views;

namespace TbsCrossPlatform.Services
{
    public class WaitingService : IWaitingService
    {
        private LoadingWindow _loadingWindow;

        public WaitingService()
        {
            _loadingWindow = new();
            _loadingWindow.ViewModel.Message = "Please wait . . .";
        }

        public void Close()
        {
            //_loadingWindow.ViewModel.CanClose = true;
            _loadingWindow.Hide();
        }

        public async Task Show()
        {
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await _loadingWindow.ShowDialog(desktop.MainWindow);
            }
        }
    }
}