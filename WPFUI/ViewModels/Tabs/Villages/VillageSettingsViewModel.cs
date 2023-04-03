using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Microsoft.Win32;
using ReactiveUI;
using Splat;
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

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class VillageSettingsViewModel : VillageTabBaseViewModel
    {
        private readonly IUpgradeBuildingHelper _upgradeBuildingHelper;

        public VillageSettingsViewModel()
        {
            _upgradeBuildingHelper = Locator.Current.GetService<IUpgradeBuildingHelper>();
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);

            this.WhenAnyValue(x => x.Settings.UpgradeTroop).Subscribe(LoadUpgradeTroop);
        }

        protected override void Init(int villageId)
        {
            LoadData(villageId);
        }

        private void LoadData(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSettings.Find(villageId);
            RxApp.MainThreadScheduler.Schedule(() => Settings.CopyFrom(settings));
        }

        private async Task SaveTask()
        {
            if (!Settings.IsValidate()) return;
            _waitingWindow.Show("saving village's settings");

            await Task.Run(() =>
            {
                var villageId = VillageId;
                Save(villageId);
                var accountId = AccountId;
                TaskBasedSetting(villageId, accountId);
            });
            _waitingWindow.Close();

            MessageBox.Show("Saved.");
        }

        private void ImportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
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
                    var villageId = VillageId;
                    setting.VillageId = villageId;
                    context.Update(setting);
                    context.SaveChanges();
                    LoadData(villageId);
                    var accountId = AccountId;
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
            var villageId = VillageId;
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
            for (var i = 0; i < TroopUpgrade.Count; i++)
            {
                var troop = TroopUpgrade[i];
                Settings.UpgradeTroop[i] = troop.IsChecked;
            }
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(index);
            Settings.CopyTo(setting);
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
                        _upgradeBuildingHelper.RemoveFinishedCB(villageId);
                        using var context = _contextFactory.CreateDbContext();
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
                var tasks = list.OfType<RefreshVillage>();
                if (Settings.IsAutoRefresh)
                {
                    if (!tasks.Any(x => x.VillageId == villageId))
                    {
                        _taskManager.Add(accountId, _taskFactory.GetRefreshVillageTask(villageId, accountId));
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

        private void LoadUpgradeTroop(bool[] upgradeTroop)
        {
            if (!IsActive) return;
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                using var context = _contextFactory.CreateDbContext();
                var troops = context.VillagesTroops.Where(x => x.VillageId == VillageId).ToArray();
                TroopUpgrade.Clear();
                for (var i = 0; i < troops.Length; i++)
                {
                    var troop = troops[i];
                    TroopUpgrade.Add(new TroopInfoCheckBox
                    {
                        Troop = (TroopEnums)troop.Id,
                        IsChecked = upgradeTroop[i],
                    });
                }
            });
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }

        public VillageSetting Settings { get; } = new();
        public ObservableCollection<TroopInfoCheckBox> TroopUpgrade { get; } = new();
    }
}