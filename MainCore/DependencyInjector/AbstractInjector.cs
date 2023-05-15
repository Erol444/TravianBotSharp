using FluentMigrator.Runner;
using MainCore.Migrations;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MainCore.DependencyInjector
{
    public abstract class AbstractInjector : IInjector
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        private IServiceCollection ConfigureService(IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connectionString));
            services.AddSingleton<IChromeManager, ChromeManager>();
            services.AddSingleton<IRestClientManager, RestClientManager>();
            services.AddSingleton<IUseragentManager, UseragentManager>();
            services.AddSingleton<IEventManager, EventManager>();
            services.AddSingleton<ITimerManager, TimerManager>();
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddSingleton<IPlanManager, PlanManager>();
            services.AddSingleton<ILogManager, LogManager>();

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString(_connectionString)
                    .WithMigrationsIn(typeof(Farming).Assembly));
            return services;
        }

        protected abstract IServiceCollection ConfigureParser(IServiceCollection services);

        protected abstract IServiceCollection ConfigureHelper(IServiceCollection services);

        public IServiceCollection Configure(IServiceCollection services)
        {
            ConfigureService(services);
            ConfigureParser(services);
            ConfigureHelper(services);

            return services;
        }
    }
}