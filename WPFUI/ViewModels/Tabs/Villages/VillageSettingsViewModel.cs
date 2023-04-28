using MainCore;
using MainCore.Enums;
using MainCore.Helper.Interface;
using Microsoft.Win32;
using ReactiveUI;
using Splat;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class VillageSettingsViewModel : VillageTabBaseViewModel
    {
        private readonly IUpgradeBuildingHelper _upgradeBuildingHelper;

        private bool _useHeroRes;

        public bool UseHeroRes
        {
            get => _useHeroRes;
            set => this.RaiseAndSetIfChanged(ref _useHeroRes, value);
        }

        private bool _ignoreRoman;

        public bool IgnoreRoman
        {
            get => _ignoreRoman;
            set => this.RaiseAndSetIfChanged(ref _ignoreRoman, value);
        }

        public CheckBoxWithInputViewModel AutoComplete { get; } = new();
        public CheckBoxWithInputViewModel WatchAds { get; } = new();

        private bool _isAutoRefresh;

        public bool IsAutoRefresh
        {
            get => _isAutoRefresh;
            set => this.RaiseAndSetIfChanged(ref _isAutoRefresh, value);
        }

        public ToleranceViewModel AutoRefresh { get; } = new();

        public CheckBoxWithInputViewModel AutoNPCCrop { get; } = new();
        public CheckBoxWithInputViewModel AutoNPCResource { get; } = new();
        private bool _isAutoNPCOverflow;

        public bool IsAutoNPCOverflow
        {
            get => _isAutoNPCOverflow;
            set => this.RaiseAndSetIfChanged(ref _isAutoNPCOverflow, value);
        }

        public ResourcesViewModel RatioNPC { get; } = new();
        public TroopTrainingSelectorViewModel BarrackTraining { get; } = new();
        public TroopTrainingSelectorViewModel StableTraining { get; } = new();
        public TroopTrainingSelectorViewModel WorkshopTraining { get; } = new();

        public VillageSettingsViewModel()
        {
            _upgradeBuildingHelper = Locator.Current.GetService<IUpgradeBuildingHelper>();
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
        }

        protected override void Init(int villageId)
        {
            LoadData(villageId);
        }

        private void LoadData(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSettings.Find(villageId);

            UseHeroRes = settings.IsUseHeroRes;
            IgnoreRoman = settings.IsIgnoreRomanAdvantage;
            AutoComplete.IsChecked = settings.IsInstantComplete;
            AutoComplete.Value = settings.InstantCompleteTime;
            WatchAds.IsChecked = settings.IsAdsUpgrade;
            WatchAds.Value = settings.AdsUpgradeTime;

            IsAutoRefresh = settings.IsAutoRefresh;
            AutoRefresh.MainValue = (settings.AutoRefreshTimeMax + settings.AutoRefreshTimeMin) / 2;
            AutoRefresh.ToleranceValue = (settings.AutoRefreshTimeMax - settings.AutoRefreshTimeMin) / 2;

            AutoNPCCrop.IsChecked = settings.IsAutoNPC;
            AutoNPCCrop.Value = settings.AutoNPCPercent;
            AutoNPCResource.IsChecked = settings.IsAutoNPCWarehouse;
            AutoNPCResource.Value = settings.AutoNPCWarehousePercent;
            IsAutoNPCOverflow = settings.IsNPCOverflow;
            RatioNPC.Wood = settings.AutoNPCWood;
            RatioNPC.Clay = settings.AutoNPCClay;
            RatioNPC.Iron = settings.AutoNPCIron;
            RatioNPC.Crop = settings.AutoNPCCrop;

            var account = context.Villages.Find(villageId);
            var accountInfo = context.AccountsInfo.Find(account.AccountId);
            var tribe = accountInfo.Tribe;

            BarrackTraining.LoadData(tribe.GetInfantryTroops().Select(x => new TroopInfo(x)), (TroopEnums)settings.BarrackTroop);
            StableTraining.LoadData(tribe.GetCavalryTroops().Select(x => new TroopInfo(x)), (TroopEnums)settings.StableTroop);
            WorkshopTraining.LoadData(tribe.GetSiegeTroops().Select(x => new TroopInfo(x)), (TroopEnums)settings.WorkshopTroop);
        }

        private async Task SaveTask()
        {
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
        }

        private void TaskBasedSetting(int villageId, int accountId)
        {
            //var list = _taskManager.GetList(accountId);
            //{
            //    var tasks = list.Where(x => x is InstantUpgrade);
            //    if (Settings.IsInstantComplete)
            //    {
            //        if (!tasks.Any())
            //        {
            //            _upgradeBuildingHelper.RemoveFinishedCB(villageId);
            //            using var context = _contextFactory.CreateDbContext();
            //            var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
            //            var count = currentBuildings.Count(x => x.Level != -1);
            //            if (count > 0)
            //            {
            //                _taskManager.Add(accountId, new InstantUpgrade(villageId, accountId));
            //            }
            //        }
            //    }
            //    else
            //    {
            //        foreach (var item in tasks)
            //        {
            //            _taskManager.Remove(accountId, item);
            //        }
            //    }
            //}
            //{
            //    var tasks = list.OfType<RefreshVillage>();
            //    if (Settings.IsAutoRefresh)
            //    {
            //        if (!tasks.Any(x => x.VillageId == villageId))
            //        {
            //            _taskManager.Add(accountId, _taskFactory.GetRefreshVillageTask(villageId, accountId));
            //        }
            //    }
            //    else
            //    {
            //        var updateTasks = tasks.Where(x => x.VillageId == villageId);
            //        foreach (var item in updateTasks)
            //        {
            //            _taskManager.Remove(accountId, item);
            //        }
            //    }
            //}
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
    }
}