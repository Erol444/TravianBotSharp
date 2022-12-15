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
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.Tabs.Villages;
using WPFUI.ViewModels.Uc.BuildView;
using WPFUI.ViewModels.Uc.FarmingView;
using WPFUI.ViewModels.Uc.MainView;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace WPFUI
{
    public static class AppBoostrapper
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
                })
                .Build();
            var container = host.Services;
            container.UseMicrosoftDependencyResolver();
            return container;
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
            services.AddSingleton<VersionViewModel>();
            services.AddSingleton<WaitingViewModel>();

            services.AddSingleton<AddAccountsViewModel>();
            services.AddSingleton<AddAccountViewModel>();
            services.AddSingleton<EditAccountViewModel>();
            services.AddSingleton<DebugViewModel>();
            services.AddSingleton<FarmingViewModel>();
            services.AddSingleton<HeroViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<VillagesViewModel>();

            services.AddSingleton<BuildViewModel>();
            services.AddSingleton<InfoViewModel>();
            services.AddSingleton<NPCViewModel>();
            services.AddSingleton<VillageSettingsViewModel>();
            services.AddSingleton<VillageTroopsViewModel>();
            services.AddSingleton<MarketViewModel>();

            services.AddSingleton<SelectorViewModel>();
            return services;
        }

        public static IServiceCollection ConfigureUcViewModel(this IServiceCollection services)
        {
            // main view
            services.AddSingleton<MainTabPanelViewModel>();
            services.AddSingleton<MainButtonPanelViewModel>();
            services.AddSingleton<AccountListViewModel>();

            // farming view
            services.AddSingleton<FarmListViewModel>();
            services.AddSingleton<FarmContentViewModel>();

            // build view
            services.AddSingleton<BuildingListViewModel>();
            services.AddSingleton<CurrentBuildingListViewModel>();
            services.AddSingleton<QueueListViewModel>();
            services.AddSingleton<BuildButtonPanelViewModel>();
            services.AddSingleton<NormalBuildViewModel>();
            services.AddSingleton<ResourcesBuildViewModel>();
            return services;
        }
    }
}