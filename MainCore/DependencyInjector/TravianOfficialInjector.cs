using MainCore.Helper.Implementations.TravianOfficial;
using MainCore.Helper.Interface;
using MainCore.Parsers.Implementations.TravianOfficial;
using MainCore.Parsers.Interface;
using MainCore.Services.Implementations.TaskFactories;
using MainCore.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace MainCore.DependencyInjector
{
    public class TravianOfficialInjector : AbstractInjector
    {
        protected override IServiceCollection ConfigureHelper(IServiceCollection services)
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
            return services;
        }

        protected override IServiceCollection ConfigureFactory(IServiceCollection services)
        {
            services.AddSingleton<ITaskFactory, TravianOfficialTaskFactory>();
            return services;
        }
    }
}