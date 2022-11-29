using FluentMigrator.Runner;
using MainCore.Helper.Implementations;
using MainCore.Helper.Interface;
using MainCore.Migrations;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServerModuleCore;
using ServerModuleCore.Parser;
using System;
using Microsoft.Extensions.Hosting;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat;
using ILogManager = MainCore.Services.Interface.ILogManager;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore;
using TravianOfficialCore.Parsers;

#elif TTWARS

using TTWarsCore;
using TTWarsCore.Parsers;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore
{
    public static class AppBootstrapper
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceProvider Init()
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();

                    services.ConfigureServices();
                    services.ConfigureParser();
                    services.ConfigureHelper();
                })
                .Build();

            return host.Services;
        }

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

        public static IServiceCollection ConfigureParser(this IServiceCollection services)
        {
            services.AddSingleton<IBuildingTabParser, BuildingTabParser>();
            services.AddSingleton<IFarmListParser, FarmListParser>();
            services.AddSingleton<IHeroSectionParser, HeroSectionParser>();
            services.AddSingleton<INavigationBarParser, NavigationBarParser>();
            services.AddSingleton<IRightBarParser, RightBarParser>();
            services.AddSingleton<IStockBarParser, StockBarParser>();
            services.AddSingleton<ISubTabParser, SubTabParser>();
            services.AddSingleton<ISystemPageParser, SystemPageParser>();
            services.AddSingleton<IVillageCurrentlyBuildingParser, VillageCurrentlyBuildingParser>();
            services.AddSingleton<IVillageFieldParser, VillageFieldParser>();
            services.AddSingleton<IVillageInfrastructureParser, VillageInfrastructureParser>();
            services.AddSingleton<IVillagesTableParser, VillagesTableParser>();
            services.AddSingleton<IUpgradingPageParser, UpgradingPageParser>();
            services.AddSingleton<IUrlValidator, UrlValidator>();
            return services;
        }

        public static IServiceCollection ConfigureHelper(this IServiceCollection services)
        {
            services.AddSingleton<IAccessHelper, AccessHelper>();
            services.AddSingleton<IBuildingsHelper, BuildingsHelper>();
            services.AddSingleton<ICheckHelper, CheckHelper>();
            services.AddSingleton<IClickHelper, ClickHelper>();
            services.AddSingleton<IGithubHelper, GithubHelper>();
            services.AddSingleton<IHeroHelper, HeroHelper>();
            services.AddSingleton<INavigateHelper, NavigateHelper>();
            services.AddSingleton<IUpdateHelper, UpdateHelper>();
            services.AddSingleton<IUpgradeBuildingHelper, UpgradeBuildingHelper>();
            return services;
        }
    }
}