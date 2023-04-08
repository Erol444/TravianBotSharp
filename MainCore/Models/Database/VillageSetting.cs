using System.Text;

namespace MainCore.Models.Database
{
    public class VillageSetting
    {
        public int VillageId { get; set; }
        public bool IsUseHeroRes { get; set; }
        public bool IsIgnoreRomanAdvantage { get; set; }
        public bool IsInstantComplete { get; set; }
        public int InstantCompleteTime { get; set; }

        public bool IsAdsUpgrade { get; set; }
        public int AdsUpgradeTime { get; set; }

        public bool IsAutoRefresh { get; set; }
        public int AutoRefreshTimeMin { get; set; }
        public int AutoRefreshTimeMax { get; set; }

        public bool IsAutoNPC { get; set; }
        public bool IsAutoNPCWarehouse { get; set; }
        public bool IsNPCOverflow { get; set; }
        public int AutoNPCPercent { get; set; }
        public int AutoNPCWarehousePercent { get; set; }
        public int AutoNPCWood { get; set; }
        public int AutoNPCClay { get; set; }
        public int AutoNPCIron { get; set; }
        public int AutoNPCCrop { get; set; }

        public bool IsUpgradeTroop { get; set; }
        public string UpgradeTroop { get; set; }

        public bool[] GetTroopUpgrade()
        {
            var result = new bool[10];
            if (string.IsNullOrEmpty(UpgradeTroop))
                return result;
            var arr = UpgradeTroop.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                result[i] = arr[i] == "1";
            }
            return result;
        }

        public void SetTroopUpgrade(bool[] arr)
        {
            var result = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                result.Append(arr[i] ? "1" : "0");
                if (i < arr.Length - 1)
                    result.Append(',');
            }
            UpgradeTroop = result.ToString();
        }

        public bool IsTrainTroop { get; set; }
        public bool IsGreatBuilding { get; set; }
        public string TrainTroop { get; set; }

        public bool[] GetTroopTrain()
        {
            var result = new bool[10];
            if (string.IsNullOrEmpty(TrainTroop))
                return result;
            var arr = TrainTroop.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                result[i] = arr[i] == "1";
            }
            return result;
        }

        public void SetTroopTrain(bool[] arr)
        {
            var result = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                result.Append(arr[i] ? "1" : "0");
                if (i < arr.Length - 1)
                    result.Append(',');
            }
            TrainTroop = result.ToString();
        }
    }
}