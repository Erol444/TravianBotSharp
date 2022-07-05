using FluentMigrator.Runner;
using MainCore;
using MainCore.MigrationDb;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

            var contextFactory = SetupService.GetService<IDbContextFactory<AppDbContext>>();
            using (var context = contextFactory.CreateDbContext())
            {
                using var scope = SetupService.ServiceProvider.CreateScope();
                var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                if (!context.Database.EnsureCreated())
                {
                    migrationRunner.MigrateUp();
                }
            }

            var timerManager = SetupService.GetService<ITimerManager>();
            timerManager.Start();

            var logManager = SetupService.GetService<ILogManager>();
            logManager.Init();

            var versionWindow = SetupService.GetService<VersionWindow>();
            await versionWindow.ViewModel.Load();
            if (versionWindow.ViewModel.IsNewVersion) versionWindow.Show();

            var mainWindow = SetupService.GetService<MainWindow>();
            mainWindow.ViewModel.LoadData();
            mainWindow.Show();
            waitingWindow.Hide();
        }
    }
}