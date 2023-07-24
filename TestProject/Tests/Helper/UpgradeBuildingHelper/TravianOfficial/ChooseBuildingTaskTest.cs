using MainCore;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splat;
using TestProject.Mock;
using TestProject.Mock.Helper;
using TestProject.Mock.Helper.UpgradeBuildingHelper.ChooseBuildingTask;
using WPFUI;

namespace TestProject.Tests.Helper.UpgradeBuildingHelper.TravianOfficial
{
    [Ignore("Not sure how to test this")]
    [TestClass]
    public class ChooseBuildingTaskTest
    {
        private IDbContextFactory<AppDbContext> _contextFactory;

        private IPlanManager _planManager;
        private IUpgradeBuildingHelper _upgradeBuildingHelper;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            AppBoostrapper.Init(VersionEnums.TravianOfficial);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
            _planManager = Locator.Current.GetService<IPlanManager>();
            _upgradeBuildingHelper = Locator.Current.GetService<IUpgradeBuildingHelper>();

            using var context = _contextFactory.CreateDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            _planManager.Clear(FakeIdData.VillageId);
        }

        /// <summary>
        /// Normal choose building
        /// </summary>
        [TestMethod]
        public void NormalChooseTest()
        {
            {
                foreach (var task in FakePlanData.BuildingFirst)
                {
                    _planManager.Add(FakeIdData.VillageId, task);
                }

                using var context = _contextFactory.CreateDbContext();
                context.VillagesCurrentlyBuildings.AddRange(FakeCurrentBuildingData.Empty);
                context.SaveChanges();
            }

            var result = _upgradeBuildingHelper.ChooseBuilding(FakeIdData.AccountId, FakeIdData.VillageId);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(BuildingEnums.MainBuilding, result.Value.Building);
        }

        /// <summary>
        /// Normal choose but has plus account
        /// </summary>
        [TestMethod]
        public void NormalChoosePlusAccountTest()
        {
            {
                foreach (var task in FakePlanData.BuildingFirst)
                {
                    _planManager.Add(FakeIdData.VillageId, task);
                }

                using var context = _contextFactory.CreateDbContext();
                context.AccountsInfo.Add(FakeAccountInfoData.GetAccountInfo(TribeEnums.Gauls, true));
                context.VillagesCurrentlyBuildings.AddRange(FakeCurrentBuildingData.One);
                context.SaveChanges();
            }

            var result = _upgradeBuildingHelper.ChooseBuilding(FakeIdData.AccountId, FakeIdData.VillageId);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(BuildingEnums.MainBuilding, result.Value.Building);
        }

        /// <summary>
        /// has no plus account
        /// </summary>
        [TestMethod]
        public void ErrorPlusAccountTest()
        {
            {
                foreach (var task in FakePlanData.BuildingFirst)
                {
                    _planManager.Add(FakeIdData.VillageId, task);
                }

                using var context = _contextFactory.CreateDbContext();
                context.AccountsInfo.Add(FakeAccountInfoData.GetAccountInfo(TribeEnums.Gauls, false));
                context.VillagesCurrentlyBuildings.AddRange(FakeCurrentBuildingData.One);
                context.SaveChanges();
            }

            var result = _upgradeBuildingHelper.ChooseBuilding(FakeIdData.AccountId, FakeIdData.VillageId);
            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(result.HasError<BuildingQueue>());
        }

        /// <summary>
        /// Roman choose with 2 building in currently queue
        /// </summary>
        [TestMethod]
        public void RomanChooseTest()
        {
            {
                foreach (var task in FakePlanData.BuildingFirst)
                {
                    _planManager.Add(FakeIdData.VillageId, task);
                }

                using var context = _contextFactory.CreateDbContext();
                context.AccountsInfo.Add(FakeAccountInfoData.GetAccountInfo(TribeEnums.Romans, true));
                context.VillagesSettings.Add(FakeVillageSettingsData.GetVillageSetting(false));
                context.VillagesCurrentlyBuildings.AddRange(FakeCurrentBuildingData.TwoBuildingOnly);
                context.VillagesResources.AddRange(FakeVillageResourcesData.Normal);
                context.SaveChanges();
            }

            var result = _upgradeBuildingHelper.ChooseBuilding(FakeIdData.AccountId, FakeIdData.VillageId);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(BuildingEnums.Cropland, result.Value.Building);
        }

        /// <summary>
        /// Empty queue
        /// </summary>
        [TestMethod]
        public void EmptyQueueTest()
        {
            var result = _upgradeBuildingHelper.ChooseBuilding(FakeIdData.AccountId, FakeIdData.VillageId);
            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(result.HasError<Skip>());
        }

        /// <summary>
        /// Error currently queue is full (2 buildings)
        /// </summary>
        [TestMethod]
        public void FullCurrentlyQueueTest()
        {
            {
                foreach (var task in FakePlanData.BuildingFirst)
                {
                    _planManager.Add(FakeIdData.VillageId, task);
                }

                using var context = _contextFactory.CreateDbContext();
                context.AccountsInfo.Add(FakeAccountInfoData.GetAccountInfo(TribeEnums.Gauls, true));
                context.VillagesSettings.Add(FakeVillageSettingsData.GetVillageSetting(false));
                context.VillagesCurrentlyBuildings.AddRange(FakeCurrentBuildingData.Full);
                context.SaveChanges();
            }

            var result = _upgradeBuildingHelper.ChooseBuilding(FakeIdData.AccountId, FakeIdData.VillageId);
            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(result.HasError<BuildingQueue>());
        }

        /// <summary>
        /// Error currently queue is full with romans tribe (3 buildings)
        /// </summary>
        [TestMethod]
        public void FullCurrentlyQueueRomanTest()
        {
            {
                foreach (var task in FakePlanData.BuildingFirst)
                {
                    _planManager.Add(FakeIdData.VillageId, task);
                }

                using var context = _contextFactory.CreateDbContext();
                context.AccountsInfo.Add(FakeAccountInfoData.GetAccountInfo(TribeEnums.Romans, true));
                context.VillagesSettings.Add(FakeVillageSettingsData.GetVillageSetting(false));
                context.VillagesCurrentlyBuildings.AddRange(FakeCurrentBuildingData.Full);
                context.SaveChanges();
            }

            var result = _upgradeBuildingHelper.ChooseBuilding(FakeIdData.AccountId, FakeIdData.VillageId);
            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(result.HasError<BuildingQueue>());
        }
    }
}