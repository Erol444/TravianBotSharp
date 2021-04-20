using System;
using TravBotSharp.Files.Tasks.ResourcesConfiguration;

namespace TbsCore.Models.ResourceModels
{
    public class SendResourcesConfiguration
    {
        //public int TargetVillageId { get; set; } //use global setting for this,

        public bool Enabled { get; set; }
        public BalanceType BalanceType { get; set; }
        public Resources TargetLimit { get; set; } //in %
        public Resources FillLimit { get; set; } //if 50k, max fill will be 50k res

        /// <summary>
        ///     When sending resources to main village, only send above these %:
        /// </summary>
        public Resources SendResLimit { get; set; }

        public DateTime TransitArrival { get; set; }

        public void Init()
        {
            TargetLimit = new Resources();
            FillLimit = new Resources();
            SendResLimit = new Resources
            {
                Wood = 10,
                Clay = 10,
                Iron = 10,
                Crop = 10
            };
        }
    }
}

namespace TravBotSharp.Files.Tasks.ResourcesConfiguration
{
    public enum BalanceType
    {
        SendTo,
        RecieveFrom
    }
}