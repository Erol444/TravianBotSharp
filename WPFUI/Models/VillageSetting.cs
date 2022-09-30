using ReactiveUI;

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
        }

        public void CopyTo(MainCore.Models.Database.VillageSetting settings)
        {
            settings.IsUseHeroRes = IsUseHeroRes;
            settings.IsInstantComplete = IsInstantComplete;
            settings.InstantCompleteTime = int.Parse(InstantCompleteTime);
            settings.IsAdsUpgrade = IsAdsUpgrade;
            settings.AdsUpgradeTime = int.Parse(AdsUpgradeTime);
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
    }
}