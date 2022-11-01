using FluentMigrator.Runner;
using MainCore;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using UI.ViewModels.UserControls;
using UI.Views;

namespace UI.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(AccountTableViewModel accountTableViewModel, LoadingOverlayViewModel loadingOverlayViewModel, IChromeManager chromeManager, IUseragentManager useragentManager, IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IGithubService githubService) : base()
        {
            _accountTableViewModel = accountTableViewModel;
            _loadingOverlayViewModel = loadingOverlayViewModel;
            _chromeManager = chromeManager;
            _useragentManager = useragentManager;
            _planManager = planManager;
            _contextFactory = contextFactory;
            _githubService = githubService;

            InitServicesCommand = ReactiveCommand.CreateFromTask(InitServicesTask);
        }

        private async Task InitServicesTask()
        {
            _loadingOverlayViewModel.Load();
            try
            {
                _loadingOverlayViewModel.LoadingText = "Checking chromedriver.exe ...";
                await ChromeDriverInstaller.Install();
            }
            catch (Exception e)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error", e.Message);
                await messageBoxStandardWindow.Show();
            }
            {
                _loadingOverlayViewModel.LoadingText = "Loading Chrome's extension ...";
                await Task.Run(() => _chromeManager.LoadExtension());
            }
            {
                _loadingOverlayViewModel.LoadingText = "Loading useragents file ...";
                await _useragentManager.Load();
            }
            {
                _loadingOverlayViewModel.LoadingText = "Loading database file ...";
                await Task.Run(() =>
                {
                    using var context = _contextFactory.CreateDbContext();
                    using var scope = App.Container.CreateScope();
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
                });
            }
            {
                _loadingOverlayViewModel.LoadingText = "Loading buidings queue ...";
                await Task.Run(() => _planManager.Load());
            }
            {
                _loadingOverlayViewModel.LoadingText = "Checking new version ...";
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                currentVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build);
                var result = await _githubService.IsNewVersion(currentVersion);
                if (result) Locator.Current.GetService<VersionWindow>().Show();
            }
            {
                await _accountTableViewModel.LoadData();
            }
            _loadingOverlayViewModel.Unload();
        }

        private readonly AccountTableViewModel _accountTableViewModel;
        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;

        private readonly IChromeManager _chromeManager;
        private readonly IUseragentManager _useragentManager;
        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IGithubService _githubService;

        public ReactiveCommand<Unit, Unit> InitServicesCommand { get; }
    }
}