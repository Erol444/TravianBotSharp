using FluentMigrator.Runner;
using MainCore;
using MainCore.Enums;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
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
using WPFUI.ViewModels.Uc.MainView;
using WPFUI.Views;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace WPFUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IPlanManager _planManager;
        private readonly ITaskManager _taskManager;
        private readonly IChromeManager _chromeManager;
        private readonly ILogManager _logManager;
        private readonly ITimerManager _timerManager;
        private readonly IUseragentManager _useragentManager;
        private readonly IRestClientManager _restClientManager;
        private readonly WaitingOverlayViewModel _waitingOverlay;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly VersionWindow _versionWindow;
        private MainLayoutViewModel _mainLayoutViewModel;

        public MainLayoutViewModel MainLayoutViewModel
        {
            get => _mainLayoutViewModel;
            set => this.RaiseAndSetIfChanged(ref _mainLayoutViewModel, value);
        }

        public WaitingOverlayViewModel WaitingOverlay => _waitingOverlay;

        public MainWindowViewModel(IPlanManager planManager, ITaskManager taskManager, IChromeManager chromeManager, ILogManager logManager, ITimerManager timerManager, IRestClientManager restClientManager, WaitingOverlayViewModel waitingOverlay, IDbContextFactory<AppDbContext> contextFactory, IUseragentManager useragentManager)
        {
            _planManager = planManager;
            _taskManager = taskManager;
            _chromeManager = chromeManager;
            _logManager = logManager;
            _timerManager = timerManager;
            _restClientManager = restClientManager;
            _waitingOverlay = waitingOverlay;
            _contextFactory = contextFactory;
            _useragentManager = useragentManager;
            _versionWindow = new VersionWindow();
        }

        public async Task Load()
        {
            _waitingOverlay.Show("loading data");
            try
            {
                await ChromeDriverInstaller.Install();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
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
                    _logManager.Init();
                })
            };

            await Task.WhenAll(tasks);
            await _versionWindow.ViewModel.Load();

            MainLayoutViewModel = new();

            if (_versionWindow.ViewModel.IsNewVersion) _versionWindow.ViewModel.Show();
            _waitingOverlay.Close();
        }

        public async Task ClosingTask(CancelEventArgs e)
        {
            _waitingOverlay.Show("saving data");
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
                _logManager.Shutdown();
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
                    _waitingOverlay.Show("waiting current task stops");
                    await Task.Run(() =>
                    {
                        while (current.Stage != TaskStage.Waiting) { }
                    });
                    _waitingOverlay.Close();
                }
                _taskManager.UpdateAccountStatus(index, AccountStatus.Paused);
                return;
            }
        }
    }
}