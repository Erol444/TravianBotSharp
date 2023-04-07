using MainCore.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFUI;

namespace TestProject.DependencyInjector
{
    [TestClass]
    public class InitServicesVersionDependTests
    {
        [DataTestMethod, Timeout(10000)]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void TestInit(VersionEnums version, Type type)
        {
            AppBoostrapper.Init(version);
            var result = Locator.Current.GetService(type);
            Assert.IsNotNull(result);
        }

        private static IEnumerable<object[]> GetTestData()
        {
            var namespaces = new List<string>()
            {
                "MainCore.Parser.Interface",
                "MainCore.Helper.Interface",
            };

            var result = new List<object[]>();

            foreach (var ns in namespaces)
            {
                var typelist = GetTypesInNamespace(ns);

                var versionList = Enum.GetValues<VersionEnums>();
                foreach (var version in versionList)
                {
                    result.AddRange(typelist.Select(x => new object[] { version, x }).ToList());
                }
            }

            return result;
        }

        private static Type[] GetTypesInNamespace(string nameSpace)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsInterface && (t.Namespace?.Contains(nameSpace) ?? false)).ToArray();
        }
    }
}