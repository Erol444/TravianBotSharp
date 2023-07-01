using MainCore.DependencyInjector;
using Microsoft.Extensions.DependencyInjection;
using WPFUI.Store;
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.Tabs.Villages;
using WPFUI.ViewModels.Uc;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.DependencyInjector
{
    public class UIInjector : IInjector
    {
        public IServiceCollection Configure(IServiceCollection services)
        {
            ConfigureViewModel(services);
            ConfigureUcViewModel(services);
            ConfigureStore(services);

            return services;
        }

        private static IServiceCollection ConfigureViewModel(IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<VersionOverlayViewModel>();
            services.AddSingleton<WaitingOverlayViewModel>();

            services.AddSingleton<AddAccountsViewModel>();
            services.AddSingleton<AddAccountViewModel>();
            services.AddSingleton<EditAccountViewModel>();
            services.AddSingleton<DebugViewModel>();
            services.AddSingleton<FarmingViewModel>();
            services.AddSingleton<HeroViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<VillagesViewModel>();
            services.AddSingleton<NoAccountViewModel>();

            services.AddSingleton<NoVillageViewModel>();
            services.AddSingleton<BuildViewModel>();
            services.AddSingleton<InfoViewModel>();
            services.AddSingleton<NPCViewModel>();
            services.AddSingleton<VillageSettingsViewModel>();
            services.AddSingleton<VillageTroopsViewModel>();

            return services;
        }

        private static IServiceCollection ConfigureUcViewModel(IServiceCollection services)
        {
            // main view
            services.AddSingleton<MainLayoutViewModel>();
            return services;
        }

        private static IServiceCollection ConfigureStore(IServiceCollection services)
        {
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<VillageNavigationStore>();
            services.AddSingleton<SelectedItemStore>();
            return services;
        }
    }
}