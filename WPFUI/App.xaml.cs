using FluentMigrator.Runner;
using MainCore;
using MainCore.Miragations;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Views;
using EventManager = MainCore.Services.EventManager;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ServiceProvider _provider;
        public static IServiceProvider Provider => _provider;

        public static T GetService<T>() => Provider.GetRequiredService<T>();

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            _provider = new ServiceCollection().ConfigureServices().BuildServiceProvider();

            var waitingWindow = GetService<WaitingWindow>();
            waitingWindow.ViewModel.Show("loading data");

            var tasks = new List<Task>
            {
                ChromeDriverInstaller.Install(),
                Task.Run(() =>
                {
                    var chromeManager = GetService<IChromeManager>();
                    chromeManager.LoadExtension();
                }),

                Task.Run(async () =>
                {
                    var useragentManager = GetService<IUseragentManager>();
                    await useragentManager.Load();
                }),
                Task.Run(() =>
                {
                    var contextFactory = GetService<IDbContextFactory<AppDbContext>>();
                    using var context = contextFactory.CreateDbContext();
                    using var scope = Provider.CreateScope();
                    var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    if (!context.Database.EnsureCreated())
                    {
                        migrationRunner.MigrateUp();
                        context.UpdateDatabase();
                    }
                    var planManager = GetService<IPlanManager>();
                    planManager.Load();
                }),

                Task.Run(() =>
                {
                    var logManager = GetService<ILogManager>();
                    logManager.Init();
                })
            };

            await Task.WhenAll(tasks);

            var versionWindow = new VersionWindow();
            await versionWindow.ViewModel.Load();
            if (versionWindow.ViewModel.IsNewVersion) versionWindow.Show();

            var mainWindow = GetService<MainWindow>();
            mainWindow.Show();
            waitingWindow.ViewModel.Close();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _provider.Dispose();
        }
    }

    public static class DependencyInjectionContainer
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<WaitingWindow>();

            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));
            services.AddSingleton<IChromeManager, ChromeManager>();
            services.AddSingleton<IRestClientManager, RestClientManager>();
            services.AddSingleton<IUseragentManager, UseragentManager>();
            services.AddSingleton<IEventManager, EventManager>();
            services.AddSingleton<ITimerManager, TimerManager>();
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddSingleton<IPlanManager, PlanManager>();
            services.AddSingleton<ILogManager, LogManager>();

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(_connectionString)
                .ScanIn(typeof(Farming).Assembly).For.Migrations());
            return services;
        }
    }
}