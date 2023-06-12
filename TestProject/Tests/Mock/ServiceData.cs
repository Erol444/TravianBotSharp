using MainCore;
using MainCore.Enums;
using MainCore.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFUI;
using WPFUI.ViewModels.Uc.MainView;

namespace TestProject.Tests.Mock
{
    public class ServiceData
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
            typeof(ILogManager),
        };

        public static IEnumerable<object[]> GetCoreService()
        {
            var result = new List<object[]>();

            var typelist = GetInterfaceInNamespace("MainCore.Services.Interface");

            var ignoreList = new Type[]
            {
                typeof(IChromeBrowser),
            };
            var typeListExcept = typelist.Except(ignoreList);
            var versionList = GetVersionEnums();
            foreach (var version in versionList)
            {
                result.AddRange(typeListExcept.Select(x => new object[] { version, x }).ToList());
            }
            return result;
        }

        public static IEnumerable<object[]> GetVersionService()
        {
            var namespaces = new List<string>()
            {
                "MainCore.Parsers.Interface",
                "MainCore.Helper.Interface",
            };

            var result = new List<object[]>();

            foreach (var ns in namespaces)
            {
                var typelist = GetInterfaceInNamespace(ns);

                var versionList = GetVersionEnums();
                foreach (var version in versionList)
                {
                    result.AddRange(typelist.Select(x => new object[] { version, x }).ToList());
                }
            }

            return result;
        }

        public static IEnumerable<object[]> GetUIService()
        {
            var namespaces = new List<string>()
            {
                "WPFUI.ViewModels",
                "WPFUI.ViewModels.Uc.MainView",
                "WPFUI.ViewModels.Tabs",
                "WPFUI.ViewModels.Tabs.Villages",
                "WPFUI.Store",
            };
            var ignoreList = new Type[]
            {
                typeof(TabHeaderViewModel),
            };

            var result = new List<object[]>();

            foreach (var ns in namespaces)
            {
                var typelist = GetClassInNamespace(ns);

                var typeListExcept = typelist.Except(ignoreList);
                var versionList = GetVersionEnums();
                foreach (var version in versionList)
                {
                    result.AddRange(typeListExcept.Select(x => new object[] { version, x }).ToList());
                }
            }

            return result;
        }

        public static IEnumerable<object[]> GetVersions()
        {
            var versionList = GetVersionEnums();
            return versionList.Select(x => new object[] { x });
        }

        private static Type[] GetInterfaceInNamespace(string nameSpace)
        {
            var forceMainCore = typeof(AppDbContext).Assembly;
            var forceUI = typeof(App).Assembly;
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsInterface && !string.IsNullOrEmpty(t.Namespace) && t.Namespace.Equals(nameSpace)).ToArray();
        }

        private static Type[] GetClassInNamespace(string nameSpace)
        {
            var forceMainCore = typeof(AppDbContext).Assembly;
            var forceUI = typeof(App).Assembly;
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && !string.IsNullOrEmpty(t.Namespace) && t.Namespace.Equals(nameSpace) && t.IsPublic).ToArray();
        }

        private static VersionEnums[] GetVersionEnums()
        {
            return Enum.GetValues<VersionEnums>();
        }
    }
}