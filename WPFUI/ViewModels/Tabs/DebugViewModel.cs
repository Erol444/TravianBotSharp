using MainCore.Models.Runtime;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class DebugViewModel : AccountTabViewModelBase
    {
        private const string discordUrl = "https://discord.gg/DVPV4gesCz";

        public DebugViewModel()
        {
            _eventManager.TaskUpdate += OnTasksUpdate;
            _eventManager.LogUpdate += OnLogsUpdate;

            GetHelpCommand = ReactiveCommand.Create(GetHelpTask);
            LogFolderCommand = ReactiveCommand.Create(LogFolderTask);
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        protected override void Reload(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int accountId)
        {
            OnTasksUpdate(accountId);

            Logs.Clear();
            var logs = _logManager.GetLog(accountId);
            foreach (var log in logs)
            {
                Logs.Add(log);
            }
        }

        private void GetHelpTask()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = discordUrl,
                UseShellExecute = true
            });
        }

        private void LogFolderTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.Accounts.Find(_selectorViewModel.Account.Id);
            var name = info.Username;
            Process.Start(new ProcessStartInfo(Path.Combine(AppContext.BaseDirectory, "logs"))
            {
                UseShellExecute = true
            });
        }

        private void OnTasksUpdate(int accountId)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Tasks.Clear();
                var tasks = _taskManager.GetList(accountId);
                foreach (var item in tasks)
                {
                    if (item is null) continue;
                    Tasks.Add(new TaskModel()
                    {
                        Task = item.Name,
                        ExecuteAt = item.ExecuteAt,
                        Stage = item.Stage,
                    });
                }
            });
        }

        private void OnLogsUpdate(int accountId, LogMessage logMessage)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Logs.Insert(0, logMessage);
            });
        }

        public ObservableCollection<TaskModel> Tasks { get; } = new();

        public ObservableCollection<LogMessage> Logs { get; } = new();
        public ReactiveCommand<Unit, Unit> GetHelpCommand { get; }
        public ReactiveCommand<Unit, Unit> LogFolderCommand { get; }
    }
}