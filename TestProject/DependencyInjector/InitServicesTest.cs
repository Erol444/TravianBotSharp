using MainCore.Services.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFUI;

namespace TestProject.DependencyInjector
{
    [TestClass]
    public class InitServicesTest
    {
        private static readonly List<Type> serviceType = new()
        {
            typeof(IChromeManager),
            typeof(IRestClientManager),
            typeof(IUseragentManager),
            typeof(IEventManager),
            typeof(ITimerManager),
            typeof(ITaskManager),
            typeof(IPlanManager),
            typeof(MainCore.Services.Interface.ILogManager),
        };

        [TestInitialize]
        public void Init()
        {
            AppBoostrapper.Init();
        }

        [DataTestMethod, Timeout(10000)]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void TestInit(Type type)
        {
            var result = Locator.Current.GetService(type);
            Assert.IsNotNull(result);
        }

        private static IEnumerable<object[]> GetTestData()
        {
            return serviceType.Select(x => new object[] { x }).ToArray();
        }
    }
}