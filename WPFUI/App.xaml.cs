using Microsoft.Extensions.DependencyInjection;
using Splat;
using System;
using System.Windows;
using WPFUI.ViewModels;
using WPFUI.Views;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Container { get; private set; }

        private readonly MainWindow mainWindow;

        public App()
        {
            Container = AppBoostrapper.Init();

            mainWindow = new MainWindow()
            {
                ViewModel = Locator.Current.GetService<MainWindowViewModel>(),
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}