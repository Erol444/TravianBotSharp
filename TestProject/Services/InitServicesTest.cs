using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splat;
using System;
using System.Collections.Generic;
using WPFUI;
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.Tabs.Villages;
using WPFUI.ViewModels.Uc;

namespace TestProject.Services
{
    [TestClass]
    public class InitServicesTest
    {
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
            return new[]
            {
                new object[] {typeof(MainWindowViewModel)},
                new object[] {typeof(VersionViewModel) },
                new object[] {typeof(WaitingViewModel) },
                new object[] {typeof(ButtonPanelViewModel) },
                new object[] {typeof(FarmListControllerViewModel) },

                new object[] {typeof(AddAccountsViewModel) },
                new object[] {typeof(AddAccountViewModel) },
                new object[] {typeof(DebugViewModel) },
                new object[] {typeof(FarmingViewModel) },
                new object[] {typeof(GeneralViewModel) },
                new object[] {typeof(HeroViewModel) },
                new object[] {typeof(SettingsViewModel) },
                new object[] {typeof(VillagesViewModel) },

                new object[] {typeof(BuildViewModel) },
                new object[] {typeof(InfoViewModel) },
                new object[] {typeof(NPCViewModel) },
                new object[] {typeof(VillageSettingsViewModel) },
                new object[] {typeof(VillageTroopsViewModel) },

                new object[] {typeof(SelectorViewModel) },
            };
        }
    }
}