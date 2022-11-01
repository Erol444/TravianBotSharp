using FluentMigrator.Runner;
using MainCore;
using MainCore.Migrations;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using UI.ViewModels;
using UI.ViewModels.UserControls;
using UI.Views;
using UI.Views.Tabs;
using ILogManager = MainCore.Services.Interface.ILogManager;

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
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();

                    services.ConfigureServices();
                    services.ConfigureUcViewModel();
                    services.ConfigureViewModel();
                    services.ConfigureView();
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
            services.AddSingleton<IEventManager, EventManager>();
            services.AddSingleton<ITimerManager, TimerManager>();
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddSingleton<IPlanManager, PlanManager>();
            services.AddSingleton<ILogManager, LogManager>();

            services.AddSingleton<IGithubService, GithubService>();
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString(_connectionString)
                    .ScanIn(typeof(Farming).Assembly).For.Migrations());

            return services;
        }

        public static IServiceCollection ConfigureViewModel(this IServiceCollection services)
        {
            services.AddSingleton<AccountViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<VersionViewModel>();
            return services;
        }

        public static IServiceCollection ConfigureUcViewModel(this IServiceCollection services)
        {
            services.AddSingleton<LoadingOverlayViewModel>();
            services.AddSingleton<AccountTableViewModel>();
            services.AddSingleton<ButtonsPanelViewModel>();
            services.AddSingleton<TabPanelViewModel>();
            return services;
        }

        public static IServiceCollection ConfigureView(this IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<VersionWindow>();
            services.AddSingleton<NoAccountTab>();
            services.AddSingleton<AddAccountTab>();

            return services;
        }
    }
}