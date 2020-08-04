using System;

namespace TravBotSharp.Files.Models.ResourceModels
{
    public class MerchantsUnderWay
    {
        public TransitType Transit { get; set; }
        public Resources Resources { get; set; }
        public DateTime Arrival { get; set; }
        public int TargetVillageId { get; set; }
        public int RepeatTimes { get; set; }
    }
    public enum TransitType
    {
        Returning, // Merchants from this village coming back
        Incoming, // Merchants from other villages sending resources to this one
        Outgoin // Merchants from this village sending to other village
    }
}
