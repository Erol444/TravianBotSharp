using MainCore.Enums;
using MainCore.Tasks.UpdateTasks;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc;

namespace WPFUI.ViewModels.Tabs
{
    public class SettingsViewModel : AccountTabBaseViewModel
    {
        private TribeComboBox _selectedTribe;

        public TribeComboBox SelectedTribe
        {
            get => _selectedTribe;
            set => this.RaiseAndSetIfChanged(ref _selectedTribe, value);
        }

        public ObservableCollection<TribeComboBox> Tribes { get; }
        public ToleranceViewModel ClickDelay { get; } = new();
        public ToleranceViewModel TaskDelay { get; } = new();
        public ToleranceViewModel WorkTime { get; } = new();
        public ToleranceViewModel SleepTime { get; } = new();

        private bool _isSleepBetweenProxyChanging;

        public bool IsSleepBetweenProxyChanging
        {
            get => _isSleepBetweenProxyChanging;
            set => this.RaiseAndSetIfChanged(ref _isSleepBetweenProxyChanging, value);
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

        private bool _IsAutoStartAdventure;

        public bool IsAutoStartAdventure
        {
            get => _IsAutoStartAdventure;
            set => this.RaiseAndSetIfChanged(ref _IsAutoStartAdventure, value);
        }

        public SettingsViewModel()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);

            Tribes = new(Enum.GetValues<TribeEnums>().Skip(1).Where(x => x != TribeEnums.Nature && x != TribeEnums.Natars).Select(x => new TribeComboBox() { Tribe = x }).ToList());
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.AccountsSettings.Find(index);
            var info = context.AccountsInfo.Find(index);

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                SelectedTribe = Tribes.FirstOrDefault(x => x.Tribe == info.Tribe);

                IsSleepBetweenProxyChanging = settings.IsSleepBetweenProxyChanging;
                IsDontLoadImage = settings.IsDontLoadImage;
                IsMinimized = settings.IsMinimized;
                IsAutoStartAdventure = settings.IsAutoAdventure;
            });

            ClickDelay.LoadData(settings.ClickDelayMin, settings.ClickDelayMax);
            TaskDelay.LoadData(settings.TaskDelayMin, settings.TaskDelayMax);
            WorkTime.LoadData(settings.WorkTimeMin, settings.WorkTimeMax);
            SleepTime.LoadData(settings.SleepTimeMin, settings.SleepTimeMax);
        }

        private async Task SaveTask()
        {
            _waitingWindow.Show("saving account's settings");

            await Task.Run(() =>
            {
                var accountId = AccountId;
                Save(accountId);
                TaskBasedSetting(accountId);
            });
            _waitingWindow.Close();

            MessageBox.Show("Saved.");
        }

        private void ImportTask()
        {
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
                    var accountId = AccountId;
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

        private void Save(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountSetting = context.AccountsSettings.Find(index);

            accountSetting.IsSleepBetweenProxyChanging = IsSleepBetweenProxyChanging;
            accountSetting.IsDontLoadImage = IsDontLoadImage;
            accountSetting.IsMinimized = IsMinimized;
            accountSetting.IsAutoAdventure = IsAutoStartAdventure;

            (accountSetting.ClickDelayMin, accountSetting.ClickDelayMax) = ClickDelay.GetData();
            (accountSetting.TaskDelayMin, accountSetting.TaskDelayMax) = TaskDelay.GetData();
            (accountSetting.WorkTimeMin, accountSetting.WorkTimeMax) = WorkTime.GetData();
            (accountSetting.SleepTimeMin, accountSetting.SleepTimeMax) = SleepTime.GetData();

            context.Update(accountSetting);
            var accountInfo = context.AccountsInfo.Find(index);
            accountInfo.Tribe = SelectedTribe?.Tribe ?? TribeEnums.Any;
            context.Update(accountInfo);
            context.SaveChanges();
        }

        private void TaskBasedSetting(int index)
        {
            var tasks = _taskManager.GetList(index);
            var task = tasks.OfType<UpdateAdventures>().FirstOrDefault();
            if (IsAutoStartAdventure)
            {
                if (task is null)
                {
                    _taskManager.Add(index, _taskFactory.GetUpdateAdventuresTask(index));
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

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
    }
}