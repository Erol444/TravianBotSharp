using MainCore;
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

            IsSendExcessResources = settings.IsSendExcessResources;
            SendExcessWood = settings.SendExcessWood.ToString();
            SendExcessClay = settings.SendExcessClay.ToString();
            SendExcessIron = settings.SendExcessIron.ToString();
            SendExcessCrop = settings.SendExcessCrop.ToString();
            SendExcessToX = settings.SendExcessToX.ToString();
            SendExcessToY = settings.SendExcessToY.ToString();

            IsGetMissingResources = settings.IsGetMissingResources;
            GetMissingWood = settings.GetMissingWood.ToString();
            GetMissingClay = settings.GetMissingClay.ToString();
            GetMissingIron = settings.GetMissingIron.ToString();
            GetMissingCrop = settings.GetMissingCrop.ToString();
            SendFromX = settings.SendFromX.ToString();
            SendFromY = settings.SendFromY.ToString();
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

            settings.IsSendExcessResources = IsSendExcessResources;
            settings.SendExcessWood = int.Parse(SendExcessWood);
            settings.SendExcessClay = int.Parse(SendExcessClay);
            settings.SendExcessIron = int.Parse(SendExcessIron);
            settings.SendExcessCrop = int.Parse(SendExcessCrop);
            settings.SendExcessToX = int.Parse(SendExcessToX);
            settings.SendExcessToY = int.Parse(SendExcessToY);

            settings.IsGetMissingResources = IsGetMissingResources;
            settings.GetMissingWood = int.Parse(GetMissingWood);
            settings.GetMissingClay = int.Parse(GetMissingClay);
            settings.GetMissingIron = int.Parse(GetMissingIron);
            settings.GetMissingCrop = int.Parse(GetMissingCrop);
            settings.SendFromX = int.Parse(SendFromX);
            settings.SendFromY = int.Parse(SendFromY);

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
            if (!SendExcessWood.IsNumeric())
            {
                MessageBox.Show("Auto SendOutResources wood is not a number.", "Warning");
                return false;
            }
            if (!SendExcessClay.IsNumeric())
            {
                MessageBox.Show("Auto NPC clay is not a number.", "Warning");
                return false;
            }
            if (!SendExcessIron.IsNumeric())
            {
                MessageBox.Show("Auto NPC iron is not a number.", "Warning");
                return false;
            }
            if (!SendExcessCrop.IsNumeric())
            {
                MessageBox.Show("Auto NPC crop is not a number.", "Warning");
                return false;
            }
            if (SendExcessToX[0] == '-')
            {
                var positiveString = SendExcessToX.Remove(0, 1);
                if (!positiveString.IsNumeric())
                {
                    MessageBox.Show("X coorinate is not a number.", "Warning");
                    return false;
                }
            }
            else
            {
                if (!SendExcessToX.IsNumeric())
                {
                    MessageBox.Show("X coorinate is not a number.", "Warning");
                    return false;
                }
            }
            if (SendExcessToY[0] == '-')
            {
                var positiveString = SendExcessToY.Remove(0, 1);
                if (!positiveString.IsNumeric())
                {
                    MessageBox.Show("Y coorinate is not a number.", "Warning");
                    return false;
                }
            }
            else
            {
                if (!SendExcessToY.IsNumeric())
                {
                    MessageBox.Show("Y coorinate is not a number.", "Warning");
                    return false;
                }
            }
            if (SendFromX[0] == '-')
            {
                var positiveString = SendFromX.Remove(0, 1);
                if (!positiveString.IsNumeric())
                {
                    MessageBox.Show("X coorinate is not a number.", "Warning");
                    return false;
                }
            }
            else
            {
                if (!SendFromX.IsNumeric())
                {
                    MessageBox.Show("X coorinate is not a number.", "Warning");
                    return false;
                }
            }
            if (SendFromY[0] == '-')
            {
                var positiveString = SendFromY.Remove(0, 1);
                if (!positiveString.IsNumeric())
                {
                    MessageBox.Show("Y coorinate is not a number.", "Warning");
                    return false;
                }
            }
            else
            {
                if (!SendFromY.IsNumeric())
                {
                    MessageBox.Show("Y coorinate is not a number.", "Warning");
                    return false;
                }
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

        private bool _isSendExcessResources;

        public bool IsSendExcessResources
        {
            get => _isSendExcessResources;
            set => this.RaiseAndSetIfChanged(ref _isSendExcessResources, value);
        }

        private string _sendExcessWood;

        public string SendExcessWood
        {
            get => _sendExcessWood;
            set => this.RaiseAndSetIfChanged(ref _sendExcessWood, value);
        }

        private string _sendExcessClay;

        public string SendExcessClay
        {
            get => _sendExcessClay;
            set => this.RaiseAndSetIfChanged(ref _sendExcessClay, value);
        }

        private string _sendExcessIron;

        public string SendExcessIron
        {
            get => _sendExcessIron;
            set => this.RaiseAndSetIfChanged(ref _sendExcessIron, value);
        }

        private string _sendExcessCrop;

        public string SendExcessCrop
        {
            get => _sendExcessCrop;
            set => this.RaiseAndSetIfChanged(ref _sendExcessCrop, value);
        }
        private string _sendExcessToX;

        public string SendExcessToX
        {
            get => _sendExcessToX;
            set => this.RaiseAndSetIfChanged(ref _sendExcessToX, value);
        }

        private string _sendExcessToY;

        public string SendExcessToY
        {
            get => _sendExcessToY;
            set => this.RaiseAndSetIfChanged(ref _sendExcessToY, value);
        }
        private bool _isGetMissingResources;

        public bool IsGetMissingResources
        {
            get => _isGetMissingResources;
            set => this.RaiseAndSetIfChanged(ref _isGetMissingResources, value);
        }

        private string _getMissingWood;

        public string GetMissingWood
        {
            get => _getMissingWood;
            set => this.RaiseAndSetIfChanged(ref _getMissingWood, value);
        }

        private string _getMissingClay;

        public string GetMissingClay
        {
            get => _getMissingClay;
            set => this.RaiseAndSetIfChanged(ref _getMissingClay, value);
        }

        private string _getMissingIron;

        public string GetMissingIron
        {
            get => _getMissingIron;
            set => this.RaiseAndSetIfChanged(ref _getMissingIron, value);
        }

        private string _getMissingCrop;

        public string GetMissingCrop
        {
            get => _getMissingCrop;
            set => this.RaiseAndSetIfChanged(ref _getMissingCrop, value);
        }
        private string _sendFromX;

        public string SendFromX
        {
            get => _sendFromX;
            set => this.RaiseAndSetIfChanged(ref _sendFromX, value);
        }

        private string _sendFromY;

        public string SendFromY
        {
            get => _sendFromY;
            set => this.RaiseAndSetIfChanged(ref _sendFromY, value);
        }
    }
}