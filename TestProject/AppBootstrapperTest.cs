using MainCore;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerModuleCore;
using ServerModuleCore.Parser;
using Splat;
using System;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace TestProject
{
    [TestClass]
    public class AppBootstrapperTest
    {
        private IServiceProvider Container;

        [ClassInitialize]
        public void Init()
        {
            Container = AppBootstrapper.Init();
        }

        [TestMethod]
        public void TestServices()
        {
            Assert.IsNotNull(Locator.Current.GetService<IDbContextFactory<AppDbContext>>());
            Assert.IsNotNull(Locator.Current.GetService<IChromeManager>());
            Assert.IsNotNull(Locator.Current.GetService<IRestClientManager>());
            Assert.IsNotNull(Locator.Current.GetService<IUseragentManager>());
            Assert.IsNotNull(Locator.Current.GetService<IEventManager>());
            Assert.IsNotNull(Locator.Current.GetService<ITimerManager>());
            Assert.IsNotNull(Locator.Current.GetService<ITaskManager>());
            Assert.IsNotNull(Locator.Current.GetService<IPlanManager>());
            Assert.IsNotNull(Locator.Current.GetService<ILogManager>());
        }

        [TestMethod]
        public void TestParser()
        {
            Assert.IsNotNull(Locator.Current.GetService<IBuildingTabParser>());
            Assert.IsNotNull(Locator.Current.GetService<IFarmListParser>());
            Assert.IsNotNull(Locator.Current.GetService<IHeroSectionParser>());
            Assert.IsNotNull(Locator.Current.GetService<INavigationBarParser>());
            Assert.IsNotNull(Locator.Current.GetService<IRightBarParser>());
            Assert.IsNotNull(Locator.Current.GetService<IStockBarParser>());
            Assert.IsNotNull(Locator.Current.GetService<ISubTabParser>());
            Assert.IsNotNull(Locator.Current.GetService<ISystemPageParser>());
            Assert.IsNotNull(Locator.Current.GetService<IVillageCurrentlyBuildingParser>());
            Assert.IsNotNull(Locator.Current.GetService<IVillageFieldParser>());
            Assert.IsNotNull(Locator.Current.GetService<IVillageInfrastructureParser>());
            Assert.IsNotNull(Locator.Current.GetService<IVillagesTableParser>());
            Assert.IsNotNull(Locator.Current.GetService<IUpgradingPageParser>());
            Assert.IsNotNull(Locator.Current.GetService<IUrlValidator>());
        }

        [TestMethod]
        public void TestHelper()
        {
            Assert.IsNotNull(Locator.Current.GetService<IAccessHelper>());
            Assert.IsNotNull(Locator.Current.GetService<IBuildingsHelper>());
            Assert.IsNotNull(Locator.Current.GetService<ICheckHelper>());
            Assert.IsNotNull(Locator.Current.GetService<IClickHelper>());
            Assert.IsNotNull(Locator.Current.GetService<IGithubHelper>());
            Assert.IsNotNull(Locator.Current.GetService<IHeroHelper>());
            Assert.IsNotNull(Locator.Current.GetService<INavigateHelper>());
            Assert.IsNotNull(Locator.Current.GetService<IUpdateHelper>());
            Assert.IsNotNull(Locator.Current.GetService<IUpgradeBuildingHelper>());
        }
    }
}