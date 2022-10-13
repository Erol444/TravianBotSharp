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
    public class SettingsViewModel : AccountTabBaseViewModel, ITabPage
    {
        public SettingsViewModel() : base()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
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

        protected override void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.AccountsSettings.Find(index);
            Settings.CopyFrom(settings);
        }

        private async Task SaveTask()
        {
            if (!Settings.IsVaild()) return;
            _waitingWindow.ViewModel.Show("saving account's settings");

            await Task.Run(() =>
            {
                var accountId = CurrentAccount.Id;
                Save(accountId);
                TaskBasedSetting(accountId);
            });
            _waitingWindow.ViewModel.Close();

            MessageBox.Show("Saved.");
        }

        private void ImportTask()
        {
            if (!Settings.IsVaild()) return;

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(CurrentAccount.Id);
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
                    var accountId = CurrentAccount.Id;
                    setting.AccountId = accountId;
                    context.Update(setting);
                    context.SaveChanges();
                    LoadData(accountId);
                    TaskBasedSetting(accountId);
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
            var account = context.Accounts.Find(CurrentAccount.Id);
            if (account is null) return;
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{account.Username}_settings.tbs",
            };

            var accountSetting = context.AccountsSettings.Find(CurrentAccount.Id);
            var jsonString = JsonSerializer.Serialize(accountSetting);
            if (svd.ShowDialog() == true)
            {
                File.WriteAllText(svd.FileName, jsonString);
            }
        }

        private void Save(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountSetting = context.AccountsSettings.Find(index);
            Settings.CopyTo(accountSetting);
            Settings.CopyFrom(accountSetting);
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
                    _taskManager.Update(index);
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