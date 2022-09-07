namespace MainCore.Models.Database
{
    public class VillageSetting
    {
        public int VillageId { get; set; }
        public bool IsUseHeroRes { get; set; }
        public bool IsInstantComplete { get; set; }
        public int InstantCompleteTime { get; set; }
        public bool IsAdsUpgrade { get; set; }
        public int AdsUpgradeTime { get; set; }
    }
}