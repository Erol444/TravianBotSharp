using MainCore.Helper;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace WPFUI.Models
{
    public class VillageSetting : ReactiveObject
    {
        public VillageSetting()
        {
            this.WhenAnyValue(vm => vm.IsAutoNPC).Where(x => x).Subscribe(_ => IsAutoNPCWarehouse = false);
            this.WhenAnyValue(vm => vm.IsAutoNPCWarehouse).Where(x => x).Subscribe(_ => IsAutoNPC = false);
        }

        public void CopyFrom(MainCore.Models.Database.VillageSetting settings)
        {
            IsUseHeroRes = settings.IsUseHeroRes;
            IsIgnoreRomanAdvantage = settings.IsIgnoreRomanAdvantage;
            IsInstantComplete = settings.IsInstantComplete;
            InstantCompleteTime = settings.InstantCompleteTime.ToString();
            IsAdsUpgrade = settings.IsAdsUpgrade;
            AdsUpgradeTime = settings.AdsUpgradeTime.ToString();

            IsAutoRefresh = settings.IsAutoRefresh;
            AutoRefreshTime = $"{(settings.AutoRefreshTimeMin + settings.AutoRefreshTimeMax) / 2}";
            AutoRefreshTimeTolerance = $"{(settings.AutoRefreshTimeMax - settings.AutoRefreshTimeMin) / 2}";

            IsAutoNPC = settings.IsAutoNPC;
            IsAutoNPCWarehouse = settings.IsAutoNPCWarehouse;
            IsNPCOverflow = settings.IsNPCOverflow;

            AutoNPCPercent = settings.AutoNPCPercent.ToString();
            AutoNPCWarehousePercent = settings.AutoNPCWarehousePercent.ToString();
            AutoNPCWood = settings.AutoNPCWood.ToString();
            AutoNPCClay = settings.AutoNPCClay.ToString();
            AutoNPCIron = settings.AutoNPCIron.ToString();
            AutoNPCCrop = settings.AutoNPCCrop.ToString();

            IsUpgradeTroop = settings.IsUpgradeTroop;
            UpgradeTroop = settings.GetTroopUpgrade();
        }

        public void CopyTo(MainCore.Models.Database.VillageSetting settings)
        {
            settings.IsUseHeroRes = IsUseHeroRes;
            settings.IsIgnoreRomanAdvantage = IsIgnoreRomanAdvantage;
            settings.IsInstantComplete = IsInstantComplete;
            settings.InstantCompleteTime = int.Parse(InstantCompleteTime);
            if (settings.InstantCompleteTime < 0) settings.InstantCompleteTime = 0;
            settings.IsAdsUpgrade = IsAdsUpgrade;
            settings.AdsUpgradeTime = int.Parse(AdsUpgradeTime);
            if (settings.AdsUpgradeTime < 0) settings.AdsUpgradeTime = 0;

            settings.IsAutoRefresh = IsAutoRefresh;
            var autoRefreshTime = int.Parse(AutoRefreshTime);
            var autoRefreshTimeTolerance = int.Parse(AutoRefreshTimeTolerance);
            settings.AutoRefreshTimeMin = autoRefreshTime - autoRefreshTimeTolerance;
            if (settings.AutoRefreshTimeMin < 0) settings.AutoRefreshTimeMin = 0;
            settings.AutoRefreshTimeMax = autoRefreshTime + autoRefreshTimeTolerance;

            settings.IsAutoNPC = IsAutoNPC;
            settings.IsAutoNPCWarehouse = IsAutoNPCWarehouse;
            settings.IsNPCOverflow = IsNPCOverflow;
            settings.AutoNPCPercent = int.Parse(AutoNPCPercent);
            settings.AutoNPCWarehousePercent = int.Parse(AutoNPCWarehousePercent);
            if (settings.AutoRefreshTimeMin < 4) settings.AutoRefreshTimeMin = 4;
            settings.AutoNPCWood = int.Parse(AutoNPCWood);
            settings.AutoNPCClay = int.Parse(AutoNPCClay);
            settings.AutoNPCIron = int.Parse(AutoNPCIron);
            settings.AutoNPCCrop = int.Parse(AutoNPCCrop);

            settings.IsUpgradeTroop = IsUpgradeTroop;
            settings.SetTroopUpgrade(UpgradeTroop);
        }

        public bool IsValidate()
        {
            if (!InstantCompleteTime.IsNumeric())
            {
                MessageBox.Show("Instant complete time is not a number.", "Warning");
                return false;
            }
            if (!AdsUpgradeTime.IsNumeric())
            {
                MessageBox.Show("Ads upgrade time is not a number.", "Warning");
                return false;
            }
            if (!AutoRefreshTime.IsNumeric())
            {
                MessageBox.Show("Auto refresh time is not a number.", "Warning");
                return false;
            }
            if (!AutoRefreshTimeTolerance.IsNumeric())
            {
                MessageBox.Show("Auto refresh time tolerance is not a number.", "Warning");
                return false;
            }
            if (!AutoNPCPercent.IsNumeric())
            {
                MessageBox.Show("Auto NPC percent is not a number.", "Warning");
                return false;
            }
            if (!AutoNPCWood.IsNumeric())
            {
                MessageBox.Show("Auto NPC wood is not a number.", "Warning");
                return false;
            }
            if (!AutoNPCClay.IsNumeric())
            {
                MessageBox.Show("Auto NPC clay is not a number.", "Warning");
                return false;
            }
            if (!AutoNPCIron.IsNumeric())
            {
                MessageBox.Show("Auto NPC iron is not a number.", "Warning");
                return false;
            }
            if (!AutoNPCCrop.IsNumeric())
            {
                MessageBox.Show("Auto NPC crop is not a number.", "Warning");
                return false;
            }

            return true;
        }

        private bool _isUseHeroRes;

        public bool IsUseHeroRes
        {
            get => _isUseHeroRes;
            set => this.RaiseAndSetIfChanged(ref _isUseHeroRes, value);
        }

        private bool _isIgnoreRomanAdvantage;

        public bool IsIgnoreRomanAdvantage
        {
            get => _isIgnoreRomanAdvantage;
            set => this.RaiseAndSetIfChanged(ref _isIgnoreRomanAdvantage, value);
        }

        private bool _isInstantComplete;

        public bool IsInstantComplete
        {
            get => _isInstantComplete;
            set => this.RaiseAndSetIfChanged(ref _isInstantComplete, value);
        }

        private string _instantCompleteTime;

        public string InstantCompleteTime
        {
            get => _instantCompleteTime;
            set => this.RaiseAndSetIfChanged(ref _instantCompleteTime, value);
        }

        private bool _isAdsUpgrade;

        public bool IsAdsUpgrade
        {
            get => _isAdsUpgrade;
            set => this.RaiseAndSetIfChanged(ref _isAdsUpgrade, value);
        }

        private string _adsUpgradeTime;

        public string AdsUpgradeTime
        {
            get => _adsUpgradeTime;
            set => this.RaiseAndSetIfChanged(ref _adsUpgradeTime, value);
        }

        private bool _isAutoRefresh;

        public bool IsAutoRefresh
        {
            get => _isAutoRefresh;
            set => this.RaiseAndSetIfChanged(ref _isAutoRefresh, value);
        }

        private string _autoRefreshTime;

        public string AutoRefreshTime
        {
            get => _autoRefreshTime;
            set => this.RaiseAndSetIfChanged(ref _autoRefreshTime, value);
        }

        private string _atuoRefreshTimeTolerance;

        public string AutoRefreshTimeTolerance
        {
            get => _atuoRefreshTimeTolerance;
            set => this.RaiseAndSetIfChanged(ref _atuoRefreshTimeTolerance, value);
        }

        private bool _isAutoNPC;

        public bool IsAutoNPC
        {
            get => _isAutoNPC;
            set => this.RaiseAndSetIfChanged(ref _isAutoNPC, value);
        }

        private bool _isAutoNPCWarehouse;

        public bool IsAutoNPCWarehouse
        {
            get => _isAutoNPCWarehouse;
            set => this.RaiseAndSetIfChanged(ref _isAutoNPCWarehouse, value);
        }

        private bool _isNPCOverflow;

        public bool IsNPCOverflow
        {
            get => _isNPCOverflow;
            set => this.RaiseAndSetIfChanged(ref _isNPCOverflow, value);
        }

        private string _autoNPCPercent;

        public string AutoNPCPercent
        {
            get => _autoNPCPercent;
            set => this.RaiseAndSetIfChanged(ref _autoNPCPercent, value);
        }

        private string _autoNPCWarehousePercent;

        public string AutoNPCWarehousePercent
        {
            get => _autoNPCWarehousePercent;
            set => this.RaiseAndSetIfChanged(ref _autoNPCWarehousePercent, value);
        }

        private string _autoNPCWood;

        public string AutoNPCWood
        {
            get => _autoNPCWood;
            set => this.RaiseAndSetIfChanged(ref _autoNPCWood, value);
        }

        private string _autoNPCClay;

        public string AutoNPCClay
        {
            get => _autoNPCClay;
            set => this.RaiseAndSetIfChanged(ref _autoNPCClay, value);
        }

        private string _autoNPCIron;

        public string AutoNPCIron
        {
            get => _autoNPCIron;
            set => this.RaiseAndSetIfChanged(ref _autoNPCIron, value);
        }

        private string _autoNPCCrop;

        public string AutoNPCCrop
        {
            get => _autoNPCCrop;
            set => this.RaiseAndSetIfChanged(ref _autoNPCCrop, value);
        }

        private bool _isUpgradeTroop;

        public bool IsUpgradeTroop
        {
            get => _isUpgradeTroop;
            set => this.RaiseAndSetIfChanged(ref _isUpgradeTroop, value);
        }

        private bool[] _upgradeTroop;

        public bool[] UpgradeTroop
        {
            get => _upgradeTroop;
            set => this.RaiseAndSetIfChanged(ref _upgradeTroop, value);
        }
    }
}