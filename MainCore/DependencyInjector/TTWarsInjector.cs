using MainCore.Helper.Implementations.TTWars;
using MainCore.Helper.Interface;
using MainCore.Parsers.Implementations.TTWars;
using MainCore.Parsers.Interface;
using MainCore.Services.Implementations.TaskFactories;
using MainCore.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace MainCore.DependencyInjector
{
    public class TTWarsInjector : AbstractInjector
    {
        protected override IServiceCollection ConfigureHelper(IServiceCollection services)
        {
            services.AddSingleton<IAccessHelper, AccessHelper>();
            services.AddSingleton<IGithubHelper, GithubHelper>();
            services.AddSingleton<IUpgradeBuildingHelper, UpgradeBuildingHelper>();

            services.AddTransient<ICheckHelper, CheckHelper>();
            services.AddTransient<IUpdateHelper, UpdateHelper>();
            services.AddTransient<IGeneralHelper, GeneralHelper>();
            services.AddTransient<IBuildingsHelper, BuildingsHelper>();
            services.AddTransient<ITrainTroopHelper, TrainTroopHelper>();
            services.AddTransient<ICompleteNowHelper, CompleteNowHelper>();
            services.AddSingleton<IHeroResourcesHelper, HeroResourcesHelper>();

            return services;
        }

        protected override IServiceCollection ConfigureParser(IServiceCollection services)
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
            services.AddSingleton<ITrainTroopParser, TrainTroopParser>();
            return services;
        }

        protected override IServiceCollection ConfigureFactory(IServiceCollection services)
        {
            services.AddSingleton<ITaskFactory, TTWarsTaskFactory>();
            return services;
        }
    }
}