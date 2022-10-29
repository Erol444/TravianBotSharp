using FluentMigrator.Runner;
using MainCore;
using MainCore.Services;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using UI.ViewModels.UserControls;

namespace UI.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(AccountTableViewModel accountTableViewModel, LoadingOverlayViewModel loadingOverlayViewModel, IChromeManager chromeManager, IUseragentManager useragentManager, IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager) : base()
        {
            AccountTableViewModel = accountTableViewModel;
            LoadingOverlayViewModel = loadingOverlayViewModel;
            _chromeManager = chromeManager;
            _useragentManager = useragentManager;
            _planManager = planManager;
            _contextFactory = contextFactory;

            InitServicesCommand = ReactiveCommand.CreateFromTask(InitServicesTask);
        }

        private async Task InitServicesTask()
        {
            LoadingOverlayViewModel.Load();
            try
            {
                LoadingOverlayViewModel.LoadingText = "Checking chromedriver.exe ...";
                await ChromeDriverInstaller.Install();
            }
            catch (Exception e)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error", e.Message);
                await messageBoxStandardWindow.Show();
            }
            {
                LoadingOverlayViewModel.LoadingText = "Loading chrome's extension files ...";
                await Task.Run(() => _chromeManager.LoadExtension());
            }
            {
                LoadingOverlayViewModel.LoadingText = "Loading useragents file ...";
                await _useragentManager.Load();
            }
            {
                LoadingOverlayViewModel.LoadingText = "Loading database file ...";
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
                LoadingOverlayViewModel.LoadingText = "Loading buidings queue ...";
                await Task.Run(() => _planManager.Load());
            }
            {
                // checking new version
            }
            LoadingOverlayViewModel.Unload();
        }

        public AccountTableViewModel AccountTableViewModel { get; }
        public LoadingOverlayViewModel LoadingOverlayViewModel { get; }
        private readonly IChromeManager _chromeManager;
        private readonly IUseragentManager _useragentManager;
        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ReactiveCommand<Unit, Unit> InitServicesCommand { get; }
    }
}