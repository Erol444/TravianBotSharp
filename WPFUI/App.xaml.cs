using FluentMigrator.Runner;
using MainCore;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Views;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Container { get; private set; }

        private readonly MainWindow mainWindow;
        private readonly WaitingWindow waitingWindow;
        private readonly VersionWindow versionWindow;

        public App()
        {
            Container = AppBoostrapper.Init();

            mainWindow = new MainWindow();
            waitingWindow = new WaitingWindow();
            versionWindow = new VersionWindow();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            waitingWindow.ViewModel.Show("loading data");
            try
            {
                await ChromeDriverInstaller.Install();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    var chromeManager = Locator.Current.GetService<IChromeManager>();
                    chromeManager.LoadExtension();
                }),

                Task.Run(async () =>
                {
                    var useragentManager = Locator.Current.GetService<IUseragentManager>();
                    await useragentManager.Load();
                }),
                Task.Run(() =>
                {
                    var contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
                    using var context = contextFactory.CreateDbContext();
                    using var scope = Container.CreateScope();
                    var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    if (!context.Database.EnsureCreated())
                    {
                        migrationRunner.MigrateUp();
                        context.UpdateDatabase();
                    }
                    else
                    {
                        context.AddVersionInfo();
                    }
                    var planManager = Locator.Current.GetService<IPlanManager>();
                    planManager.Load();
                }),

                Task.Run(() =>
                {
                    var logManager = Locator.Current.GetService<ILogManager>();
                    logManager.Init();
                })
            };

            await Task.WhenAll(tasks);

            await versionWindow.ViewModel.Load();

            mainWindow.ViewModel.Show();
            waitingWindow.ViewModel.Close();

            if (versionWindow.ViewModel.IsNewVersion) versionWindow.ViewModel.Show();
        }
    }
}