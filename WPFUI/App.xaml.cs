//using FluentMigrator.Runner;
using MainCore;

//using MainCore.MigrationDb;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
        private static IServiceProvider _provider;
        public static IServiceProvider Provider => _provider;
        public static int AccountId { get; set; }

        public static T GetService<T>() => Provider.GetRequiredService<T>();

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _provider = new ServiceCollection().ConfigureServices().BuildServiceProvider();

            var waitingWindow = GetService<WaitingWindow>();
            waitingWindow.ViewModel.Text = "loading data";
            waitingWindow.Show();

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
                    //using var scope = Provider.CreateScope();
                    //var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    //if (!context.Database.EnsureCreated())
                    //{
                    //    migrationRunner.MigrateUp();
                    //}
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    context.Accounts.Add(new MainCore.Models.Database.Account{
                        Server = "adasd",
                        Username = "vina",
                    });
                    context.SaveChanges();
                }),

                Task.Run(() =>
                {
                    var timerManager = GetService<ITimerManager>();
                    timerManager.Start();
                }),

                Task.Run(() =>
                {
                    var logManager = GetService<ILogManager>();
                    logManager.Init();
                })
            };

            await Task.WhenAll(tasks);
            var versionWindow = GetService<VersionWindow>();

            await versionWindow.ViewModel.Load();
            if (versionWindow.ViewModel.IsNewVersion) versionWindow.Show();

            var mainWindow = GetService<MainWindow>();
            mainWindow.ViewModel.LoadData();
            mainWindow.Show();
            waitingWindow.Hide();
        }
    }

    public static class DependencyInjectionContainer
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<WaitingWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AccountWindow>();
            services.AddSingleton<AccountsWindow>();
            services.AddSingleton<VersionWindow>();
            services.AddSingleton<AccountSettingsWindow>();

            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));
            services.AddSingleton<IChromeManager, ChromeManager>();
            services.AddSingleton<IRestClientManager, RestClientManager>();
            services.AddSingleton<IUseragentManager, UseragentManager>();
            services.AddSingleton<IDatabaseEvent, DatabaseEvent>();
            services.AddSingleton<ITimerManager, TimerManager>();
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddSingleton<ILogManager, LogManager>();

            //services.AddFluentMigratorCore()
            //    .ConfigureRunner(rb => rb
            //    .AddSQLite()
            //    .WithGlobalConnectionString(_connectionString)
            //    .ScanIn(typeof(AddUserAgent).Assembly).For.Migrations());
            return services;
        }
    }
}