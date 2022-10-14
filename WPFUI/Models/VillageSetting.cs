using MainCore.Helper;
using ReactiveUI;
using System.Windows;

namespace WPFUI.Models
{
    public class VillageSetting : ReactiveObject
    {
        public void CopyFrom(MainCore.Models.Database.VillageSetting settings)
        {
            IsUseHeroRes = settings.IsUseHeroRes;
            IsInstantComplete = settings.IsInstantComplete;
            InstantCompleteTime = settings.InstantCompleteTime.ToString();
            IsAdsUpgrade = settings.IsAdsUpgrade;
            AdsUpgradeTime = settings.AdsUpgradeTime.ToString();

            IsAutoRefresh = settings.IsAutoRefresh;
            AutoRefreshTime = $"{(settings.AutoRefreshTimeMin + settings.AutoRefreshTimeMax) / 2}";
            AutoRefreshTimeTolerance = $"{(settings.AutoRefreshTimeMax - settings.AutoRefreshTimeMin) / 2}";

            IsAutoNPC = settings.IsAutoNPC;
            AutoNPCPercent = settings.AutoNPCPercent.ToString();
            AutoNPCWood = settings.AutoNPCWood.ToString();
            AutoNPCClay = settings.AutoNPCClay.ToString();
            AutoNPCIron = settings.AutoNPCIron.ToString();
            AutoNPCCrop = settings.AutoNPCCrop.ToString();
        }

        public void CopyTo(MainCore.Models.Database.VillageSetting settings)
        {
            settings.IsUseHeroRes = IsUseHeroRes;
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
            settings.AutoNPCPercent = int.Parse(AutoNPCPercent);
            if (settings.AutoRefreshTimeMin < 4) settings.AutoRefreshTimeMin = 4;
            settings.AutoNPCWood = int.Parse(AutoNPCWood);
            settings.AutoNPCClay = int.Parse(AutoNPCClay);
            settings.AutoNPCIron = int.Parse(AutoNPCIron);
            settings.AutoNPCCrop = int.Parse(AutoNPCCrop);
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

        private string _autoNPCPercent;

        public string AutoNPCPercent
        {
            get => _autoNPCPercent;
            set => this.RaiseAndSetIfChanged(ref _autoNPCPercent, value);
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
    }
}