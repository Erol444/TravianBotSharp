using Avalonia.Threading;
using AvaloniaEdit.Utils;
using MainCore;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using UI.Models;
using UI.ViewModels.UserControls;
using UI.Views;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace UI.ViewModels.Tabs
{
    public class DebugViewModel : ActivatableViewModelBase
    {
        private readonly string discordUrl = "https://discord.gg/DVPV4gesCz";

        public DebugViewModel(ILogManager logManager, ITaskManager taskManager, IEventManager eventManager, IDbContextFactory<AppDbContext> contextFactory, LoadingOverlayViewModel loadingOverlayViewModel, AccountViewModel accountViewModel) : base()
        {
            _logManager = logManager;
            _taskManager = taskManager;

            _contextFactory = contextFactory;
            _loadingOverlayViewModel = loadingOverlayViewModel;
            _accountViewModel = accountViewModel;

            eventManager.TaskUpdate += OnTaskUpdate;
            eventManager.LogUpdate += OnLogUpdate;
            _accountViewModel.AccountChanged += OnAccountChanged;
            GetHelpCommand = ReactiveCommand.CreateFromTask(GetHelpTask);
        }

        private void OnAccountChanged(int accountId)
        {
            if (!IsActive) return;
            RxApp.TaskpoolScheduler.Schedule(async () => await Load(accountId));
        }

        protected override void OnActived(CompositeDisposable disposable)
        {
            base.OnActived(disposable);
            RxApp.TaskpoolScheduler.Schedule(async () => await Load(_accountViewModel.Account.Id));
        }

        private void OnLogUpdate(int accountId, LogMessage log)
        {
            if (!IsActive) return;
            if (_accountViewModel.IsAccountNotSelected) return;
            if (_accountViewModel.Account.Id != accountId) return;
            RxApp.TaskpoolScheduler.Schedule(async () => await AddLog(log));
        }

        private void OnTaskUpdate(int accountId)
        {
            if (!IsActive) return;
            if (_accountViewModel.IsAccountNotSelected) return;
            if (_accountViewModel.Account.Id != accountId) return;
            RxApp.TaskpoolScheduler.Schedule(async () => await LoadTask(accountId));
        }

        private async Task GetHelpTask()
        {
            await Avalonia.Application.Current.Clipboard.SetTextAsync(discordUrl);
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Information", "Copied url to clipboard");
            await messageBoxStandardWindow.ShowDialog(Locator.Current.GetService<MainWindow>());
        }

        private async Task Load(int accountId)
        {
            await LoadLog(accountId);
            if (_cancellationTokenSource.IsCancellationRequested) return;
            await LoadTask(accountId);
            if (_cancellationTokenSource.IsCancellationRequested) return;
        }

        private async Task LoadTask(int accountId)
        {
            var tasks = _taskManager.GetList(accountId);
            var itemTasks = new List<TaskModel>();

            await Task.Run(() =>
            {
                foreach (var item in tasks)
                {
                    if (item is null) continue;
                    if (_cancellationTokenSource.IsCancellationRequested) return;
                    itemTasks.Add(new TaskModel()
                    {
                        Task = item.Name,
                        ExecuteAt = item.ExecuteAt,
                        Stage = item.Stage,
                    });
                }
            });

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Tasks.Clear();
                Tasks.AddRange(itemTasks);
            });
        }

        private async Task LoadLog(int accountId)
        {
            var logs = _logManager.GetLog(accountId);
            var sb = new StringBuilder();

            await Task.Run(() =>
            {
                foreach (var log in logs)
                {
                    if (_cancellationTokenSource.IsCancellationRequested) return;
                    sb.Append(log);
                    sb.AppendLine();
                }
            });

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Logs = sb.ToString();
            });
        }

        private async Task AddLog(LogMessage log)
        {
            var sb = new StringBuilder(Logs);
            await Task.Run(() =>
            {
                sb.Append(log);
                sb.AppendLine();
            });
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Logs = sb.ToString();
            });
        }

        public ReactiveCommand<Unit, Unit> GetHelpCommand { get; }

        private string _logs;

        public string Logs
        {
            get => _logs;
            set => this.RaiseAndSetIfChanged(ref _logs, value);
        }

        public ObservableCollection<TaskModel> Tasks { get; } = new();

        private readonly ILogManager _logManager;
        private readonly ITaskManager _taskManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;
        private readonly AccountViewModel _accountViewModel;
    }
}