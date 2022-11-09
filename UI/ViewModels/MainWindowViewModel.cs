using FluentMigrator.Runner;
using MainCore;
using MainCore.Enums;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using UI.ViewModels.UserControls;
using UI.Views;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace UI.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(AccountTableViewModel accountTableViewModel, LoadingOverlayViewModel loadingOverlayViewModel, IChromeManager chromeManager, IUseragentManager useragentManager, IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IGithubService githubService, ITaskManager taskManager, ILogManager logManager, ITimerManager timerManager, IRestClientManager restClientManager) : base()
        {
            _accountTableViewModel = accountTableViewModel;
            _loadingOverlayViewModel = loadingOverlayViewModel;
            _chromeManager = chromeManager;
            _useragentManager = useragentManager;
            _planManager = planManager;
            _contextFactory = contextFactory;
            _githubService = githubService;
            _taskManager = taskManager;
            _logManager = logManager;
            _timerManager = timerManager;
            _restClientManager = restClientManager;
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

        private async Task Pause(int accountId)
        {
            var status = _taskManager.GetAccountStatus(accountId);

            if (status == AccountStatus.Online)
            {
                var current = _taskManager.GetCurrentTask(accountId);
                _taskManager.UpdateAccountStatus(accountId, AccountStatus.Pausing);
                if (current is not null)
                {
                    current.Cts.Cancel();
                    await Task.Run(() =>
                    {
                        while (current.Stage != TaskStage.Waiting) { }
                    });
                }
                _taskManager.UpdateAccountStatus(accountId, AccountStatus.Paused);
            }
        }

        public async Task ClosingTask(CancelEventArgs e)
        {
            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "Stop all accounts ...";
            using var context = await _contextFactory.CreateDbContextAsync();
            var accounts = await context.Accounts.ToListAsync();

            if (accounts.Any())
            {
                var pauseTasks = new List<Task>();
                foreach (var account in accounts)
                {
                    pauseTasks.Add(Pause(account.Id));
                }
                await Task.WhenAll(pauseTasks);
            }

            _loadingOverlayViewModel.LoadingText = "Save queue building ...";
            await Task.Run(() => _planManager.Save());

            _loadingOverlayViewModel.LoadingText = "Remove junk files ...";
            await Task.Run(() =>
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
                if (Directory.Exists(path)) Directory.Delete(path, true);
            });

            _loadingOverlayViewModel.LoadingText = "Shutdown logger ...";
            await Task.Run(() =>
            {
                _logManager.Shutdown();
            });

            _loadingOverlayViewModel.LoadingText = "Shutdown all Chromes ...";
            await Task.Run(() =>
            {
                _chromeManager.Shutdown();
            });

            _loadingOverlayViewModel.LoadingText = "Shutdown all technical clients ...";
            await Task.Run(() =>
            {
                _restClientManager.Shutdown();
            });

            _loadingOverlayViewModel.LoadingText = "Shutdown all timer ...";
            await Task.Run(() =>
            {
                _timerManager.Shutdown();
            });

            _loadingOverlayViewModel.Unload();
        }

        private readonly AccountTableViewModel _accountTableViewModel;
        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;

        private readonly IChromeManager _chromeManager;
        private readonly IUseragentManager _useragentManager;
        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IGithubService _githubService;
        private readonly ITaskManager _taskManager;
        private readonly ILogManager _logManager;
        private readonly ITimerManager _timerManager;
        private readonly IRestClientManager _restClientManager;

        public ReactiveCommand<Unit, Unit> InitServicesCommand { get; }
    }
}