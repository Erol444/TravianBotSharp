using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TTWarsCore;
using WPFUI.Views;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            var startupWindow = SetupService.GetService<StartupWindow>();
            startupWindow.Show();
            await ChromeDriverInstaller.Install();
            var chromeManager = SetupService.GetService<IChromeManager>();
            chromeManager.LoadExtension();
            var mainWindow = SetupService.GetService<MainWindow>();
            mainWindow.ViewModel.LoadData();
            mainWindow.Show();
            startupWindow.Hide();
        }
    }
}