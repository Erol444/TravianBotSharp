using FluentMigrator.Runner;
using MainCore;
using MainCore.Migrations;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.ViewModels;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.Tabs.Villages;
using WPFUI.ViewModels.Uc;
using WPFUI.Views;
using EventManager = MainCore.Services.Implementations.EventManager;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Container { get; private set; }

        private readonly MainWindow mainWindow;
        private readonly WaitingWindow waitingWindow;
        private readonly VersionWindow versionWindow;

        public App()
        {
            Init();

            mainWindow = new MainWindow();
            waitingWindow = new WaitingWindow();
            versionWindow = new VersionWindow();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            waitingWindow.ViewModel.Show("loading data");
            try
            {
                await ChromeDriverInstaller.Install();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    var chromeManager = Locator.Current.GetService<IChromeManager>();
                    chromeManager.LoadExtension();
                }),

                Task.Run(async () =>
                {
                    var useragentManager = Locator.Current.GetService<IUseragentManager>();
                    await useragentManager.Load();
                }),
                Task.Run(() =>
                {
                    var contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
                    using var context = contextFactory.CreateDbContext();
                    using var scope = Container.CreateScope();
                    var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    if (!context.Database.EnsureCreated())
                    {
                        migrationRunner.MigrateUp();
                        context.UpdateDatabase();
                    }
                    else
                    {
                        context.AddVersionInfo();
                    }
                    var planManager = Locator.Current.GetService<IPlanManager>();
                    planManager.Load();
                }),

                Task.Run(() =>
                {
                    var logManager = Locator.Current.GetService<ILogManager>();
                    logManager.Init();
                })
            };

            await Task.WhenAll(tasks);

            await versionWindow.ViewModel.Load();

            mainWindow.ViewModel.Show();
            waitingWindow.ViewModel.Close();

            if (versionWindow.ViewModel.IsNewVersion) versionWindow.ViewModel.Show();
        }

        private void Init()
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
                    services.ConfigureViewModel();
                })
                .Build();
            Container = host.Services;
            Container.UseMicrosoftDependencyResolver();
        }
    }

    public static class DependencyInjectionContainer
    {
        private const string _connectionString = "DataSource=TBS.db;Cache=Shared";

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
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
                .ScanIn(typeof(Farming).Assembly).For.Migrations());
            return services;
        }

        public static IServiceCollection ConfigureViewModel(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<VersionViewModel>();
            services.AddSingleton<WaitingViewModel>();

            services.AddSingleton<ButtonPanelViewModel>();
            services.AddSingleton<FarmListControllerViewModel>();

            services.AddSingleton<AddAccountsViewModel>();
            services.AddSingleton<AddAccountViewModel>();
            services.AddSingleton<DebugViewModel>();
            services.AddSingleton<FarmingViewModel>();
            services.AddSingleton<GeneralViewModel>();
            services.AddSingleton<HeroViewModel>();
            services.AddSingleton<ViewModels.Tabs.SettingsViewModel>();
            services.AddSingleton<TabItemViewModel>();
            services.AddSingleton<VillagesViewModel>();

            services.AddSingleton<BuildViewModel>();
            services.AddSingleton<InfoViewModel>();
            services.AddSingleton<NPCViewModel>();
            services.AddSingleton<ViewModels.Tabs.Villages.SettingsViewModel>();
            services.AddSingleton<ViewModels.Tabs.Villages.TroopsViewModel>();

            return services;
        }
    }
}