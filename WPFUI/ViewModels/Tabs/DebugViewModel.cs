using MainCore.Enums;
using MainCore.Models.Runtime;
using MainCore.Services;
using MainCore.Tasks.Misc;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
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

            Button = ReactiveCommand.Create(ButtonTask);
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

        private void ButtonTask()
        {
            var items = new List<(HeroItemEnums, int)>()  {
                        (HeroItemEnums.Wood, 10),
                        (HeroItemEnums.Clay, 10),
                        (HeroItemEnums.Iron, 10),
                (HeroItemEnums.Crop, 10),
                    };
            var taskEquip = new HeroEquip(23182, AccountId, items);
            _taskManager.Add(AccountId, taskEquip);
        }

        private void OnTasksUpdate(int accountId)
        {
            if (accountId != _accountId) return;
            App.Current.Dispatcher.Invoke(() =>
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

        private void OnLogsUpdate(int accountId)
        {
            if (accountId != _accountId) return;
            App.Current.Dispatcher.Invoke(() =>
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
        public ReactiveCommand<Unit, Unit> Button { get; }

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