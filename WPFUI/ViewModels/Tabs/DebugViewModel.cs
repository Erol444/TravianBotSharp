using MainCore.Models.Runtime;
using MainCore.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WPFUI.Interfaces;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class DebugViewModel : ReactiveObject, IMainTabPage
    {
        public DebugViewModel()
        {
            _taskManager = App.GetService<ITaskManager>();
            _logManager = App.GetService<ILogManager>();
            _databaseEvent = App.GetService<IEventManager>();

            _databaseEvent.TaskUpdated += OnTasksUpdate;
            _databaseEvent.LogUpdated += OnLogsUpdate;
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        public void LoadData(int accountId)
        {
            OnTasksUpdate(accountId);
            OnLogsUpdate(accountId);
        }

        private async void OnTasksUpdate(int accountId)
        {
            if (accountId != _accountId) return;
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                Tasks.Clear();
                foreach (var item in _taskManager.GetTaskList(accountId))
                {
                    Tasks.Add(new TaskModel()
                    {
                        Task = item.Name,
                        ExecuteAt = item.ExecuteAt,
                        Stage = item.Stage,
                    });
                }
            });
        }

        private async void OnLogsUpdate(int accountId)
        {
            if (accountId != _accountId) return;
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                Logs.Clear();
                foreach (var item in _logManager.GetLog(accountId))
                {
                    Logs.Add(item);
                }
            });
        }

        private readonly ILogManager _logManager;
        private readonly ITaskManager _taskManager;
        private readonly IEventManager _databaseEvent;

        public ObservableCollection<TaskModel> Tasks { get; } = new();

        public ObservableCollection<LogMessage> Logs { get; } = new();

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set
            {
                this.RaiseAndSetIfChanged(ref _accountId, value);
                LoadData(value);
            }
        }
    }
}