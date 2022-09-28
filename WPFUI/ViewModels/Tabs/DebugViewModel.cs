using MainCore.Models.Runtime;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class DebugViewModel : AccountTabBaseViewModel, ITabPage
    {
        private readonly string discordUrl = "https://discord.gg/DVPV4gesCz";

        public DebugViewModel()
        {
            _eventManager.TaskUpdated += OnTasksUpdate;
            _eventManager.LogUpdated += OnLogsUpdate;

            GetHelpCommand = ReactiveCommand.Create(GetHelpTask);
            LogFolderCommand = ReactiveCommand.Create(LogFolderTask);
        }

        public bool IsActive { get; set; }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentAccount is not null)
            {
                LoadData(CurrentAccount.Id);
            }
        }

        public void OnDeactived()
        {
            IsActive = false;
        }

        protected override void LoadData(int accountId)
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
            var info = context.Accounts.Find(CurrentAccount.Id);
            var name = info.Username;
            Process.Start(new ProcessStartInfo(Path.Combine(AppContext.BaseDirectory, "logs"))
            {
                UseShellExecute = true
            });
        }

        private void OnTasksUpdate(int accountId)
        {
            if (CurrentAccount is null) return;
            if (CurrentAccount.Id != accountId) return;

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Tasks.Clear();
                foreach (var item in _taskManager.GetList(accountId))
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
            if (CurrentAccount is null) return;
            if (CurrentAccount.Id != accountId) return;
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