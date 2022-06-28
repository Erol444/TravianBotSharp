using MainCore;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using WPFUI.Views;

namespace WPFUI
{
    public static class SetupService
    {
        private static IServiceProvider _serviceProvider;

        public static IServiceProvider Init()
        {
            var serviceProvider = new ServiceCollection().ConfigureServices()
               .BuildServiceProvider();
            _serviceProvider = serviceProvider;
            return serviceProvider;
        }

        public static T GetService<T>() => _serviceProvider.GetService<T>();
    }

    public static class DependencyInjectionContainer
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<WaitingWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AccountWindow>();
            services.AddSingleton<AccountsWindow>();

            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite("DataSource=TBS.db;Cache=Shared"));
            services.AddSingleton<IChromeManager, ChromeManager>();
            services.AddSingleton<IRestClientManager, RestClientManager>();
            services.AddSingleton<IUseragentManager, UseragentManager>();
            services.AddSingleton<DatabaseEvent>();
            return services;
        }
    }
}