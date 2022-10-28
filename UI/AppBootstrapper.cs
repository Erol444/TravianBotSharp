using MainCore;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Splat;
using System;
using ILogManager = MainCore.Services.ILogManager;

namespace UI
{
    public static class AppBootstrapper
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterService(services, resolver);
        }

        private static void RegisterService(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite(_connectionString)
               .Options;
            services.RegisterLazySingleton<IDbContextFactory<AppDbContext>>(() => new PooledDbContextFactory<AppDbContext>(options));

            services.RegisterLazySingleton<IChromeManager>(() => new ChromeManager());

            services.RegisterLazySingleton<IRestClientManager>(() => new RestClientManager(
                resolver.GetRequiredService<IDbContextFactory<AppDbContext>>()
            ));

            services.RegisterLazySingleton<IUseragentManager>(() => new UseragentManager(
                resolver.GetRequiredService<IRestClientManager>()
            ));

            services.RegisterLazySingleton(() => new EventManager());

            services.RegisterLazySingleton<ITimerManager>(() => new TimerManager(
                resolver.GetRequiredService<EventManager>()
            ));

            services.RegisterLazySingleton<IPlanManager>(() => new PlanManager(
                resolver.GetRequiredService<IDbContextFactory<AppDbContext>>()
            ));
            services.RegisterLazySingleton<ILogManager>(() => new LogManager(
                resolver.GetRequiredService<EventManager>(),
                resolver.GetRequiredService<IDbContextFactory<AppDbContext>>()
            ));

            services.RegisterLazySingleton<ITaskManager>(() => new TaskManager(
               resolver.GetRequiredService<IDbContextFactory<AppDbContext>>(),
               resolver.GetRequiredService<IChromeManager>(),
               resolver.GetRequiredService<EventManager>(),
               resolver.GetRequiredService<ILogManager>(),
               resolver.GetRequiredService<IPlanManager>(),
               resolver.GetRequiredService<IRestClientManager>()
            ));
        }

        public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
        {
            var service = resolver.GetService<TService>();
            if (service is null)
            {
                throw new InvalidOperationException($"Failed to resolve object of type {typeof(TService)}");
            }

            return service;
        }
    }
}