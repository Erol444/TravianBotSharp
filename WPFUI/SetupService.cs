using FluentMigrator.Runner;
using MainCore;
using MainCore.MigrationDb;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using WPFUI.Views;

namespace WPFUI
{
    public static class SetupService
    {
        private static IServiceProvider _serviceProvider;

        public static IServiceProvider Init()
        {
            var serviceProvider = new ServiceCollection().ConfigureServices()
               .BuildServiceProvider();
            _serviceProvider = serviceProvider;
            return serviceProvider;
        }

        public static IServiceProvider ServiceProvider { get => _serviceProvider; }

        public static T GetService<T>() => _serviceProvider.GetRequiredService<T>();

        public static void Shutdown()
        {
            var timerManager = GetService<ITimerManager>();
            timerManager.Dispose();
            var restClientManager = GetService<IRestClientManager>();
            restClientManager.Dispose();
            var chromeManager = GetService<IChromeManager>();
            chromeManager.Dispose();
            var logManager = GetService<ILogManager>();
            logManager.Dispose();
        }
    }

    public static class DependencyInjectionContainer
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<WaitingWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AccountWindow>();
            services.AddSingleton<AccountsWindow>();
            services.AddSingleton<VersionWindow>();

            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite("DataSource=TBS.db;Cache=Shared"));
            services.AddSingleton<IChromeManager, ChromeManager>();
            services.AddSingleton<IRestClientManager, RestClientManager>();
            services.AddSingleton<IUseragentManager, UseragentManager>();
            services.AddSingleton<IDatabaseEvent, DatabaseEvent>();
            services.AddSingleton<ITimerManager, TimerManager>();
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddSingleton<ILogManager, LogManager>();

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString("DataSource=TBS.db;Cache=Shared")
                .ScanIn(typeof(AddUserAgent).Assembly).For.Migrations());
            return services;
        }
    }
}