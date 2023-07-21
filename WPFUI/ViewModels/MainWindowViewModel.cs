using FluentMigrator.Runner;
using MainCore;
using MainCore.Enums;
using MainCore.Helper.Interface;
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
            try
            {
                _waitingOverlay.Show("loading chrome driver");
                await Task.Run(_chromeManager.LoadDriver);

                _waitingOverlay.Show("loading chrome extension");
                await Task.Run(_chromeManager.LoadExtension);

                _waitingOverlay.Show("loading useragent data");
                await _useragentManager.Load();

                _waitingOverlay.Show("loading TBS's database");
                await Task.Run(MigrateDatabase);

                _waitingOverlay.Show("loading log system");
                await Task.Run(_logHelper.Init);

                _waitingOverlay.Show("loading main layout");
                await Task.Delay(100);
                MainLayoutViewModel = Locator.Current.GetService<MainLayoutViewModel>();
                await Task.Run(MainLayoutViewModel.LoadData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                _waitingOverlay.Close();
                return;
            }

            _waitingOverlay.Close();
            await _versionOverlay.Load();
        }

        private void MigrateDatabase()
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
        }

        private async Task PauseAccounts()
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
        }

        public async Task ClosingTask(CancelEventArgs e)
        {
            _waitingOverlay.Show("pausing accounts");
            await PauseAccounts();

            _waitingOverlay.Show("saving TBS's database");
            await Task.Run(_planManager.Save);

            _waitingOverlay.Show("deleting proxy's cache");
            await Task.Run(() =>
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
                if (Directory.Exists(path)) Directory.Delete(path, true);
            });

            _waitingOverlay.Show("saving log files");
            await Task.Run(_logHelper.Shutdown);

            _waitingOverlay.Show("shuting down chrome driver services");
            await Task.Run(_chromeManager.Shutdown);

            _waitingOverlay.Show("shuting down http clients");
            await Task.Run(_restClientManager.Shutdown);

            _waitingOverlay.Show("shuting down timer");
            await Task.Run(_timerManager.Shutdown);
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
                    await Task.Run(async () =>
                    {
                        while (current.Stage != TaskStage.Waiting)
                        {
                            current = _taskManager.GetCurrentTask(index);
                            if (current is null) return;
                            await Task.Delay(500);
                        }
                    });
                    _waitingOverlay.Close();
                }
                _taskManager.UpdateAccountStatus(index, AccountStatus.Paused);
                return;
            }
        }
    }
}