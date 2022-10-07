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

        public bool IsAutoRefresh { get; set; }
        public int AutoRefreshTimeMin { get; set; }
        public int AutoRefreshTimeMax { get; set; }

        public bool IsAutoNPC { get; set; }
        public int AutoNPCPercent { get; set; }
        public int AutoNPCWood { get; set; }
        public int AutoNPCClay { get; set; }
        public int AutoNPCIron { get; set; }
        public int AutoNPCCrop { get; set; }
    }
}