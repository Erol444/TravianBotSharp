namespace MainCore.Models.Database
{
    public class VillageMarket
    {
        public int VillageId { get; set; }
        public int OneMerchantSize { get; set; }

        public bool IsSendExcessResources { get; set; }
        public int SendExcessWood { get; set; }
        public int SendExcessClay { get; set; }
        public int SendExcessIron { get; set; }
        public int SendExcessCrop { get; set; }

        public bool IsGetMissingResources { get; set; }
        public int GetMissingWood { get; set; }
        public int GetMissingClay { get; set; }
        public int GetMissingIron { get; set; }
        public int GetMissingCrop { get; set; }

        // TODO: NPC section should be mvoed here

    }
}