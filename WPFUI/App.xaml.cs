using MainCore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Views;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<StartupWindow>();

            services.AddSingleton<IChromeManager, ChromeManager>();
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var startupWindow = _serviceProvider.GetService<StartupWindow>();
            startupWindow.Show();
            await ChromeDriverInstaller.Install();
            var chromeManager = _serviceProvider.GetService<IChromeManager>();
            chromeManager.LoadExtension();
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            startupWindow.Hide();
            mainWindow.Show();
        }
    }
}