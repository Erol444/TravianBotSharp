using MainCore;
using MainCore.Enums;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using WPFUI.DependencyInjectior;

namespace WPFUI
{
    public static class AppBoostrapper
    {
        public static IServiceProvider Init(VersionEnums version)
        {
            var coreInjector = InjectorFactory.GetInjector(version);
            var uiInjector = InjectorFactory.GetUIInjector();
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();

                    coreInjector.Configure(services);
                    uiInjector.Configure(services);
                })
                .Build();

            var container = host.Services;
            container.UseMicrosoftDependencyResolver();
            return container;
        }

        public static IServiceProvider Init()
        {
            return Init(VersionDetector.GetVersion());
        }
    }
}