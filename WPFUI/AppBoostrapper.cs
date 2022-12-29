using MainCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.Tabs.Villages;
using WPFUI.ViewModels.Uc.BuildView;
using WPFUI.ViewModels.Uc.FarmingView;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI
{
    public static class AppBoostrapper
    {
        public static IServiceProvider Init()
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();
                    services.ConfigureServices();
                    services.ConfigureHelper();
                    services.ConfigureParser();
                    services.ConfigureUcViewModel();
                    services.ConfigureViewModel();
                })
                .Build();
            var container = host.Services;
            container.UseMicrosoftDependencyResolver();
            return container;
        }
    }

    public static class DependencyInjectionUIContainer
    {
        public static IServiceCollection ConfigureViewModel(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<VersionViewModel>();
            services.AddSingleton<WaitingViewModel>();

            services.AddSingleton<AddAccountsViewModel>();
            services.AddSingleton<AddAccountViewModel>();
            services.AddSingleton<EditAccountViewModel>();
            services.AddSingleton<DebugViewModel>();
            services.AddSingleton<FarmingViewModel>();
            services.AddSingleton<HeroViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<VillagesViewModel>();

            services.AddSingleton<BuildViewModel>();
            services.AddSingleton<InfoViewModel>();
            services.AddSingleton<NPCViewModel>();
            services.AddSingleton<VillageSettingsViewModel>();
            services.AddSingleton<VillageTroopsViewModel>();

            services.AddSingleton<SelectorViewModel>();
            return services;
        }

        public static IServiceCollection ConfigureUcViewModel(this IServiceCollection services)
        {
            // main view
            services.AddSingleton<MainTabPanelViewModel>();
            services.AddSingleton<MainButtonPanelViewModel>();
            services.AddSingleton<AccountListViewModel>();

            // farming view
            services.AddSingleton<FarmListViewModel>();
            services.AddSingleton<FarmContentViewModel>();

            // build view
            services.AddSingleton<BuildingListViewModel>();
            services.AddSingleton<CurrentBuildingListViewModel>();
            services.AddSingleton<QueueListViewModel>();
            services.AddSingleton<BuildButtonPanelViewModel>();
            services.AddSingleton<NormalBuildViewModel>();
            services.AddSingleton<ResourcesBuildViewModel>();
            return services;
        }
    }
}