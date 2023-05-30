using MainCore.Helper.Implementations.TravianOfficial;
using MainCore.Helper.Interface;
using MainCore.Parsers.Implementations.TravianOfficial;
using MainCore.Parsers.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace MainCore.DependencyInjector
{
    public class TravianOfficialInjector : AbstractInjector
    {
        protected override IServiceCollection ConfigureHelper(IServiceCollection services)
        {
            services.AddSingleton<IAccessHelper, AccessHelper>();
            services.AddSingleton<IGithubHelper, GithubHelper>();
            services.AddSingleton<IDatabaseHelper, DatabaseHelper>();

            services.AddSingleton<ICheckHelper, CheckHelper>();
            services.AddTransient<IUpdateHelper, UpdateHelper>();
            services.AddSingleton<IGeneralHelper, GeneralHelper>();
            services.AddSingleton<IBuildingsHelper, BuildingsHelper>();

            services.AddSingleton<IInvalidPageHelper, InvalidPageHelper>();
            services.AddSingleton<IHeroResourcesHelper, HeroResourcesHelper>();
            services.AddTransient<ISleepHelper, SleepHelper>();
            services.AddSingleton<IRallypointHelper, RallypointHelper>();
            services.AddSingleton<INPCHelper, NPCHelper>();
            services.AddSingleton<ILoginHelper, LoginHelper>();
            services.AddTransient<ITrainTroopHelper, TrainTroopHelper>();
            services.AddSingleton<ICompleteNowHelper, CompleteNowHelper>();
            services.AddSingleton<IAdventureHelper, AdventureHelper>();
            services.AddTransient<IUpgradeBuildingHelper, UpgradeBuildingHelper>();
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
    }
}