using MainCore.Helper;
using MainCore.Tasks.Update;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs
{
    public class SettingsViewModel : AccountTabBaseViewModel, IMainTabPage
    {
        public SettingsViewModel() : base()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        protected override void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            if (context.Accounts.Find(index) is null) return;

            var accountSetting = context.AccountsSettings.Find(index);

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
            IsAutoStartAdventure = accountSetting.IsAutoAdventure;
        }

        private async Task SaveTask()
        {
            if (!CheckInput()) return;
            _waitingWindow.ViewModel.Show("saving account's settings");

            await Observable.Start(() =>
            {
                Save(AccountId);
                TaskBasedSetting(AccountId);
            }, RxApp.TaskpoolScheduler);
            _waitingWindow.ViewModel.Close();

            MessageBox.Show("Saved.");
        }

        private void ImportTask()
        {
            if (!CheckInput()) return;

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            var ofd = new OpenFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{account.Username}_settings.tbs",
            };

            if (ofd.ShowDialog() == true)
            {
                var jsonString = File.ReadAllText(ofd.FileName);
                try
                {
                    var setting = JsonSerializer.Deserialize<MainCore.Models.Database.AccountSetting>(jsonString);
                    var accountSetting = context.AccountsSettings.FirstOrDefault(x => x.AccountId == AccountId);
                    accountSetting.ClickDelayMax = setting.ClickDelayMax;
                    accountSetting.ClickDelayMin = setting.ClickDelayMin;
                    accountSetting.TaskDelayMax = setting.TaskDelayMax;
                    accountSetting.TaskDelayMin = setting.TaskDelayMin;
                    accountSetting.WorkTimeMax = setting.WorkTimeMax;
                    accountSetting.WorkTimeMin = setting.WorkTimeMin;
                    accountSetting.SleepTimeMax = setting.SleepTimeMax;
                    accountSetting.SleepTimeMin = setting.SleepTimeMin;
                    accountSetting.IsDontLoadImage = setting.IsDontLoadImage;
                    accountSetting.IsClosedIfNoTask = setting.IsClosedIfNoTask;
                    accountSetting.IsMinimized = setting.IsMinimized;
                    context.Update(accountSetting);
                    context.SaveChanges();
                    LoadData(AccountId);
                    TaskBasedSetting(AccountId);
                }
                catch
                {
                    MessageBox.Show("Invalid file.", "Warning");
                    return;
                }
            }
        }

        private void ExportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var accountSetting = context.AccountsSettings.FirstOrDefault(x => x.AccountId == AccountId);
            var jsonString = JsonSerializer.Serialize(accountSetting);
            var account = context.Accounts.Find(AccountId);
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{account.Username}_settings.tbs",
            };

            if (svd.ShowDialog() == true)
            {
                File.WriteAllText(svd.FileName, jsonString);
            }
        }

        private bool CheckInput()
        {
            if (!ClickDelay.IsNumeric())
            {
                MessageBox.Show("Click delay is non-numeric.", "Warning");
                return false;
            }
            if (!ClickDelayRange.IsNumeric())
            {
                MessageBox.Show("Click delay range is non-numeric.", "Warning");
                return false;
            }
            if (!TaskDelay.IsNumeric())
            {
                MessageBox.Show("Task delay is non-numeric.", "Warning");
                return false;
            }
            if (!TaskDelayRange.IsNumeric())
            {
                MessageBox.Show("Task delay range is non-numeric.", "Warning");
                return false;
            }
            if (!WorkTime.IsNumeric())
            {
                MessageBox.Show("Work time is non-numeric.", "Warning");
                return false;
            }
            if (!WorkTimeRange.IsNumeric())
            {
                MessageBox.Show("Work time range is non-numeric.", "Warning");
                return false;
            }
            if (!SleepTime.IsNumeric())
            {
                MessageBox.Show("Sleep time is non-numeric.", "Warning");
                return false;
            }
            if (!SleepTimeRange.IsNumeric())
            {
                MessageBox.Show("Sleep time range is non-numeric.", "Warning");
                return false;
            }
            return true;
        }

        private void Save(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountSetting = context.AccountsSettings.Find(index);

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
            accountSetting.IsAutoAdventure = IsAutoStartAdventure;

            context.Update(accountSetting);
            context.SaveChanges();
        }

        private void TaskBasedSetting(int index)
        {
            var list = _taskManager.GetList(index);
            var tasks = list.Where(x => x.GetType() == typeof(UpdateAdventures));
            if (IsAutoStartAdventure)
            {
                if (!tasks.Any())
                {
                    _taskManager.Add(index, new UpdateAdventures(index));
                }
            }
            else
            {
                foreach (var item in tasks)
                {
                    _taskManager.Remove(index, item);
                }
            }
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
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

        private bool _isAutoStartAdventure;

        public bool IsAutoStartAdventure
        {
            get => _isAutoStartAdventure;
            set => this.RaiseAndSetIfChanged(ref _isAutoStartAdventure, value);
        }
    }
}