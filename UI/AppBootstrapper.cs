using FluentMigrator.Runner;
using MainCore;
using MainCore.Migrations;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using UI.ViewModels;
using UI.ViewModels.UserControls;
using ILogManager = MainCore.Services.ILogManager;

namespace UI
{
    public static class AppBootstrapper
    {
        public static IServiceProvider Init()
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.ConfigureServices();
                    services.ConfigureUcViewModel();
                    services.ConfigureViewModel();
                })
                .Build();
            return host.Services;
        }
    }

    public static class DependencyInjectionContainer
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));
            services.AddSingleton<IChromeManager, ChromeManager>();
            services.AddSingleton<IRestClientManager, RestClientManager>();
            services.AddSingleton<IUseragentManager, UseragentManager>();
            services.AddSingleton<EventManager>();
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

        public static IServiceCollection ConfigureViewModel(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            return services;
        }

        public static IServiceCollection ConfigureUcViewModel(this IServiceCollection services)
        {
            services.AddSingleton<LoadingOverlayViewModel>();
            services.AddSingleton<AccountTableViewModel>();
            services.AddSingleton<ButtonsPanelViewModel>();
            return services;
        }
    }
}