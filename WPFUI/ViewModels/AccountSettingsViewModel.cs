﻿using MainCore;
using MainCore.Helper;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Views;

namespace WPFUI.ViewModels
{
    public class AccountSettingsViewModel : ReactiveObject
    {
        public AccountSettingsViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _waitingWindow = App.GetService<WaitingWindow>();

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            CloseCommand = ReactiveCommand.Create(CloseTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
        }

        public event Action CloseView;

        public void LoadData(int index)
        {
            _accountId = index;
            _waitingWindow.Show();
            using var context = _contextFactory.CreateDbContext();
            var accountSetting = context.AccountsSettings.FirstOrDefault(x => x.AccountId == index);
            if (accountSetting is null)
            {
                accountSetting = new()
                {
                    AccountId = index,
                    ClickDelayMin = 500,
                    ClickDelayMax = 900,
                    TaskDelayMin = 1000,
                    TaskDelayMax = 1500,
                    WorkTimeMin = 340,
                    WorkTimeMax = 380,
                    SleepTimeMin = 480,
                    SleepTimeMax = 600,
                    IsClosedIfNoTask = false,
                    IsDontLoadImage = false,
                    IsMinimized = false,
                };
                context.Add(accountSetting);
                context.SaveChanges();
            }

            ClickDelay = $"{(accountSetting.ClickDelayMin + accountSetting.ClickDelayMax) / 2}";
            ClickDelayRange = $"{(accountSetting.ClickDelayMax - accountSetting.ClickDelayMin) / 2}";
            TaskDelay = $"{(accountSetting.TaskDelayMin + accountSetting.TaskDelayMax) / 2}";
            TaskDelayRange = $"{(accountSetting.TaskDelayMax - accountSetting.TaskDelayMin) / 2}";
            WorkTime = $"{(accountSetting.WorkTimeMax + accountSetting.WorkTimeMin) / 2}";
            WorkTimeRange = $"{(accountSetting.WorkTimeMax - accountSetting.WorkTimeMin) / 2}";
            SleepTime = $"{(accountSetting.SleepTimeMax + accountSetting.SleepTimeMin) / 2}";
            SleepTimeRange = $"{(accountSetting.SleepTimeMax - accountSetting.SleepTimeMin) / 2}";
            IsDontLoadImage = accountSetting.IsDontLoadImage;
            IsClosedIfNoTask = accountSetting.IsClosedIfNoTask;
            IsMinimized = accountSetting.IsMinimized;

            var account = context.Accounts.Find(index);
            Username = account.Username;
            Server = account.Server;
            _waitingWindow.Hide();
        }

        private async Task SaveTask()
        {
            if (!ClickDelay.IsNumeric())
            {
                MessageBox.Show("Click delay is non-numeric.", "Warning");
                return;
            }
            if (!ClickDelayRange.IsNumeric())
            {
                MessageBox.Show("Click delay range is non-numeric.", "Warning");
                return;
            }
            if (!TaskDelay.IsNumeric())
            {
                MessageBox.Show("Task delay is non-numeric.", "Warning");
                return;
            }
            if (!TaskDelayRange.IsNumeric())
            {
                MessageBox.Show("Task delay range is non-numeric.", "Warning");
                return;
            }
            if (!WorkTime.IsNumeric())
            {
                MessageBox.Show("Work time is non-numeric.", "Warning");
                return;
            }
            if (!WorkTimeRange.IsNumeric())
            {
                MessageBox.Show("Work time range is non-numeric.", "Warning");
                return;
            }
            if (!SleepTime.IsNumeric())
            {
                MessageBox.Show("Sleep time is non-numeric.", "Warning");
                return;
            }
            if (!SleepTimeRange.IsNumeric())
            {
                MessageBox.Show("Sleep time range is non-numeric.", "Warning");
                return;
            }
            _waitingWindow.Show();

            await Task.Run(() =>
            {
                using var context = _contextFactory.CreateDbContext();
                var accountSetting = context.AccountsSettings.FirstOrDefault(x => x.AccountId == _accountId);
                if (accountSetting is null)
                {
                    accountSetting = new()
                    {
                        AccountId = _accountId
                    };
                }

                var clickDelay = int.Parse(ClickDelay);
                var clickDelayRange = int.Parse(ClickDelayRange);
                accountSetting.ClickDelayMin = clickDelay - clickDelayRange;
                accountSetting.ClickDelayMax = clickDelay + clickDelayRange;

                var taskDelay = int.Parse(TaskDelay);
                var taskDelayRange = int.Parse(TaskDelayRange);
                accountSetting.TaskDelayMin = taskDelay - taskDelayRange;
                accountSetting.TaskDelayMax = taskDelay + taskDelayRange;

                var workTime = int.Parse(WorkTime);
                var workTimeRange = int.Parse(WorkTimeRange);
                accountSetting.WorkTimeMin = workTime - workTimeRange;
                accountSetting.WorkTimeMax = workTime + workTimeRange;

                var sleepTime = int.Parse(SleepTime);
                var sleepTimeRange = int.Parse(SleepTimeRange);
                accountSetting.SleepTimeMin = sleepTime - sleepTimeRange;
                accountSetting.SleepTimeMax = sleepTime + sleepTimeRange;

                accountSetting.IsDontLoadImage = IsDontLoadImage;
                accountSetting.IsClosedIfNoTask = IsClosedIfNoTask;
                accountSetting.IsMinimized = IsMinimized;
                context.Update(accountSetting);
                context.SaveChanges();
            });
            _waitingWindow.Hide();

            MessageBox.Show("Saved.", "Info");
        }

        private void CloseTask()
        {
            CloseView?.Invoke();
        }

        private void ImportTask()
        {
        }

        private void ExportTask()
        {
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        private string _clickDelay;

        public string ClickDelay
        {
            get => _clickDelay;
            set => this.RaiseAndSetIfChanged(ref _clickDelay, value);
        }

        private string _clickDelayRange;

        public string ClickDelayRange
        {
            get => _clickDelayRange;
            set => this.RaiseAndSetIfChanged(ref _clickDelayRange, value);
        }

        private string _taskDelay;

        public string TaskDelay
        {
            get => _taskDelay;
            set => this.RaiseAndSetIfChanged(ref _taskDelay, value);
        }

        private string _taskDelayRange;

        public string TaskDelayRange
        {
            get => _taskDelayRange;
            set => this.RaiseAndSetIfChanged(ref _taskDelayRange, value);
        }

        private string _workTime;

        public string WorkTime
        {
            get => _workTime;
            set => this.RaiseAndSetIfChanged(ref _workTime, value);
        }

        private string _workTimeRange;

        public string WorkTimeRange
        {
            get => _workTimeRange;
            set => this.RaiseAndSetIfChanged(ref _workTimeRange, value);
        }

        private string _sleepTime;

        public string SleepTime
        {
            get => _sleepTime;
            set => this.RaiseAndSetIfChanged(ref _sleepTime, value);
        }

        private string _sleepTimeRange;

        public string SleepTimeRange
        {
            get => _sleepTimeRange;
            set => this.RaiseAndSetIfChanged(ref _sleepTimeRange, value);
        }

        private bool _isDontLoadImage;

        public bool IsDontLoadImage
        {
            get => _isDontLoadImage;
            set => this.RaiseAndSetIfChanged(ref _isDontLoadImage, value);
        }

        private bool _isMinimized;

        public bool IsMinimized
        {
            get => _isMinimized;
            set => this.RaiseAndSetIfChanged(ref _isMinimized, value);
        }

        private bool _isClosedIfNoTask;

        public bool IsClosedIfNoTask
        {
            get => _isClosedIfNoTask;
            set => this.RaiseAndSetIfChanged(ref _isClosedIfNoTask, value);
        }

        private string _username;

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        private string _server;

        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly WaitingWindow _waitingWindow;
        private int _accountId;
    }
}