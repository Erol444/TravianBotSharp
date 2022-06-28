using MainCore.Services;
using System.Windows;
using WPFUI.Views;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            SetupService.Init();

            var waitingWindow = SetupService.GetService<WaitingWindow>();
            waitingWindow.ViewModel.Text = "loading data";
            waitingWindow.Show();
            await ChromeDriverInstaller.Install();
            var chromeManager = SetupService.GetService<IChromeManager>();
            chromeManager.LoadExtension();

            var useragentManager = SetupService.GetService<IUseragentManager>();
            await useragentManager.Load();

            var mainWindow = SetupService.GetService<MainWindow>();
            mainWindow.ViewModel.LoadData();
            mainWindow.Show();
            waitingWindow.Hide();
        }
    }
}