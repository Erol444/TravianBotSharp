using MainCore.Helper;
using MainCore.Tasks.Sim;
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

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class SettingsViewModel : VillageTabBaseViewModel, ITabPage
    {
        public SettingsViewModel()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
        }

        public bool IsActive { get; set; }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentVillage is not null)
            {
                LoadData(CurrentVillage.Id);
            }
        }

        public void OnDeactived()
        {
            IsActive = false;
        }

        protected override void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSettings.Find(index);
            Settings.CopyFrom(settings);
        }

        private async Task SaveTask()
        {
            if (!Settings.IsValidate()) return;
            _waitingWindow.ViewModel.Show("saving village's settings");

            await Task.Run(() =>
            {
                var villageId = CurrentVillage.Id;
                Save(villageId);
                var accountId = CurrentAccount.Id;
                TaskBasedSetting(villageId, accountId);
            });
            _waitingWindow.ViewModel.Close();

            MessageBox.Show("Saved.");
        }

        private void ImportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(CurrentVillage.Id);
            var ofd = new OpenFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{village.Name}_settings.tbs",
            };

            if (ofd.ShowDialog() == true)
            {
                var jsonString = File.ReadAllText(ofd.FileName);
                try
                {
                    var setting = JsonSerializer.Deserialize<MainCore.Models.Database.VillageSetting>(jsonString);
                    var villageId = CurrentVillage.Id;
                    setting.VillageId = villageId;
                    context.Update(setting);
                    context.SaveChanges();
                    LoadData(villageId);
                    var accountId = CurrentAccount.Id;
                    TaskBasedSetting(villageId, accountId);
                }
                catch
                {
                    MessageBox.Show("Invalid file.", "Warning");
                }
            }
        }

        private void ExportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageId = CurrentVillage.Id;
            var setting = context.VillagesSettings.Find(villageId);
            var jsonString = JsonSerializer.Serialize(setting);
            var village = context.Villages.Find(villageId);
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{village.Name}_settings.tbs",
            };

            if (svd.ShowDialog() == true)
            {
                File.WriteAllText(svd.FileName, jsonString);
            }
        }

        private void Save(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(index);
            Settings.CopyTo(setting);
            Settings.CopyFrom(setting);
            context.Update(setting);
            context.SaveChanges();
        }

        private void TaskBasedSetting(int villageId, int accountId)
        {
            var list = _taskManager.GetList(accountId);
            {
                var tasks = list.Where(x => x is InstantUpgrade);
                if (Settings.IsInstantComplete)
                {
                    if (!tasks.Any())
                    {
                        using var context = _contextFactory.CreateDbContext();
                        UpgradeBuildingHelper.RemoveFinishedCB(context, villageId);
                        var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
                        var count = currentBuildings.Count(x => x.Level != -1);
                        if (count > 0)
                        {
                            _taskManager.Add(accountId, new InstantUpgrade(villageId, accountId));
                        }
                    }
                }
                else
                {
                    foreach (var item in tasks)
                    {
                        _taskManager.Remove(accountId, item);
                    }
                }
            }
            {
                var tasks = list.OfType<UpdateDorf1>();
                if (Settings.IsAutoRefresh)
                {
                    if (!tasks.Any(x => x.VillageId == villageId))
                    {
                        _taskManager.Add(accountId, new UpdateDorf1(villageId, accountId));
                    }
                }
                else
                {
                    var updateTasks = tasks.Where(x => x.VillageId == villageId);
                    foreach (var item in updateTasks)
                    {
                        _taskManager.Remove(accountId, item);
                    }
                }
            }
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public VillageSetting Settings { get; } = new();
    }
}