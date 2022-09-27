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
using WPFUI.Models;
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

            var settings = context.AccountsSettings.Find(index);
            Settings.CopyFrom(settings);
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
                    setting.AccountId = AccountId;
                    context.Update(setting);
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
            var account = context.Accounts.Find(AccountId);
            if (account is null) return;
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{account.Username}_settings.tbs",
            };

            var accountSetting = context.AccountsSettings.Find(AccountId);
            var jsonString = JsonSerializer.Serialize(accountSetting);
            if (svd.ShowDialog() == true)
            {
                File.WriteAllText(svd.FileName, jsonString);
            }
        }

        private bool CheckInput()
        {
            if (!Settings.ClickDelay.IsNumeric())
            {
                MessageBox.Show("Click delay is non-numeric.", "Warning");
                return false;
            }
            if (!Settings.ClickDelayRange.IsNumeric())
            {
                MessageBox.Show("Click delay range is non-numeric.", "Warning");
                return false;
            }
            if (!Settings.TaskDelay.IsNumeric())
            {
                MessageBox.Show("Task delay is non-numeric.", "Warning");
                return false;
            }
            if (!Settings.TaskDelayRange.IsNumeric())
            {
                MessageBox.Show("Task delay range is non-numeric.", "Warning");
                return false;
            }
            if (!Settings.WorkTime.IsNumeric())
            {
                MessageBox.Show("Work time is non-numeric.", "Warning");
                return false;
            }
            if (!Settings.WorkTimeRange.IsNumeric())
            {
                MessageBox.Show("Work time range is non-numeric.", "Warning");
                return false;
            }
            if (!Settings.SleepTime.IsNumeric())
            {
                MessageBox.Show("Sleep time is non-numeric.", "Warning");
                return false;
            }
            if (!Settings.SleepTimeRange.IsNumeric())
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
            Settings.CopyTo(accountSetting);
            context.Update(accountSetting);
            context.SaveChanges();
        }

        private void TaskBasedSetting(int index)
        {
            var tasks = _taskManager.GetList(index);
            var task = tasks.FirstOrDefault(x => x is UpdateAdventures);
            if (Settings.IsAutoStartAdventure)
            {
                if (task is null)
                {
                    _taskManager.Add(index, new UpdateAdventures(index));
                }
                else
                {
                    task.ExecuteAt = DateTime.Now;
                    _taskManager.Update(AccountId);
                }
            }
            else
            {
                if (task is not null) _taskManager.Remove(index, task);
            }
        }

        public AccountSetting Settings { get; } = new();
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
    }
}