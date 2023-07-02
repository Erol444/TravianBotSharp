using FluentMigrator.Runner;
using MainCore;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc;

namespace WPFUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IPlanManager _planManager;
        private readonly ITaskManager _taskManager;
        private readonly IChromeManager _chromeManager;
        private readonly ILogHelper _logHelper;
        private readonly ITimerManager _timerManager;
        private readonly IUseragentManager _useragentManager;
        private readonly IRestClientManager _restClientManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly WaitingOverlayViewModel _waitingOverlay;
        private readonly VersionOverlayViewModel _versionOverlay;

        private MainLayoutViewModel _mainLayoutViewModel;

        public MainLayoutViewModel MainLayoutViewModel
        {
            get => _mainLayoutViewModel;
            set => this.RaiseAndSetIfChanged(ref _mainLayoutViewModel, value);
        }

        public WaitingOverlayViewModel WaitingOverlay => _waitingOverlay;
        public VersionOverlayViewModel VersionOverlay => _versionOverlay;

        public MainWindowViewModel(IPlanManager planManager, ITaskManager taskManager, IChromeManager chromeManager, ILogHelper logHelper, ITimerManager timerManager, IRestClientManager restClientManager, WaitingOverlayViewModel waitingOverlay, IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager, VersionOverlayViewModel versionViewModel)
        {
            _planManager = planManager;
            _taskManager = taskManager;
            _chromeManager = chromeManager;
            _logHelper = logHelper;
            _timerManager = timerManager;
            _restClientManager = restClientManager;
            _waitingOverlay = waitingOverlay;
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _versionOverlay = versionViewModel;
        }

        public async Task Load()
        {
            _waitingOverlay.ShowCommand.Execute("loading data").Subscribe();
            try
            {
                await ChromeDriverInstaller.Install();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                _waitingOverlay.CloseCommand.Execute().Subscribe();
                return;
            }

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    _chromeManager.LoadExtension();
                }),

                Task.Run(async () =>
                {
                    await _useragentManager.Load();
                }),
                Task.Run(() =>
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
                    _planManager.Load();
                }),

                Task.Run(() =>
                {
                    _logHelper.Init();
                })
            };

            await Task.WhenAll(tasks);
            MainLayoutViewModel = Locator.Current.GetService<MainLayoutViewModel>();
            MainLayoutViewModel.LoadData();
            _versionOverlay.LoadCommand.Execute().Subscribe();
            _waitingOverlay.CloseCommand.Execute().Subscribe();
        }

        public async Task ClosingTask(CancelEventArgs e)
        {
            _waitingOverlay.ShowCommand.Execute("saving data").Subscribe();
            await Task.Run(async () =>
            {
                using var context = _contextFactory.CreateDbContext();
                var accounts = context.Accounts.ToList();

                if (accounts.Any())
                {
                    var pauseTasks = new List<Task>();
                    foreach (var account in accounts)
                    {
                        pauseTasks.Add(Pause(account.Id));
                    }
                    await Task.WhenAll(pauseTasks);
                }

                _planManager.Save();

                var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
                if (Directory.Exists(path)) Directory.Delete(path, true);
            });

            await Task.Run(() =>
            {
                _logHelper.Shutdown();
            });

            await Task.Run(() =>
            {
                _chromeManager.Shutdown();
            });

            await Task.Run(() =>
            {
                _restClientManager.Shutdown();
            });

            await Task.Run(() =>
            {
                _timerManager.Shutdown();
            });
        }

        private async Task Pause(int index)
        {
            var status = _taskManager.GetAccountStatus(index);
            if (status == AccountStatus.Paused)
            {
                _taskManager.UpdateAccountStatus(index, AccountStatus.Online);
                return;
            }

            if (status == AccountStatus.Online)
            {
                var current = _taskManager.GetCurrentTask(index);
                _taskManager.UpdateAccountStatus(index, AccountStatus.Pausing);
                if (current is not null)
                {
                    _waitingOverlay.ShowCommand.Execute("waiting current task stops").Subscribe();
                    await Task.Run(() =>
                    {
                        while (current.Stage != TaskStage.Waiting) { }
                    });
                    _waitingOverlay.CloseCommand.Execute().Subscribe();
                }
                _taskManager.UpdateAccountStatus(index, AccountStatus.Paused);
                return;
            }
        }
    }
}