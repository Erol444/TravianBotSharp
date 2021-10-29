using System.Collections.Generic;
using TbsCore.Helpers;
using TbsCore.TravianData;

namespace TbsCore.Models.TroopsModels
{
    /// <summary>
    /// For u1-u10 + Hero (optionally)
    /// </summary>
    public class TroopsBase
    {
        public TroopsBase() {}
        public TroopsBase(int[] arr, Classificator.TribeEnum? tribe)
        {
            this.Troops = arr;
            this.Tribe = tribe ?? Classificator.TribeEnum.Any;
        }

        public int[] Troops { get; set; }
        public Classificator.TribeEnum Tribe { get; set; }
        public List<Troop> GetTroopList()
        {
            var ret = new List<Troop>();
            for (int i = 0; i < 10; i++)
            {
                var troopType = (Classificator.TroopsEnum)i + 1 + (((int)this.Tribe - 1) * 10);
                ret.Add(new Troop(Troops[i], troopType));
            }
            return ret;
        }

        // Base deff against infantry + deff against cavalry
        public long TotalBaseDeff()
        {
            long totalDeff = 0;
            foreach(var troop in GetTroopList())
            {
                totalDeff += troop.Count * (TroopsData.TroopDeffenseInfrantry(troop.Type) + TroopsData.TroopDeffenseCavalry(troop.Type));
            }
            return totalDeff;
        }
    }

    public class Troop
    {
        public Troop(int cnt, Classificator.TroopsEnum type)
        {
            this.Count = cnt;
            this.Type = type;
        }
        public int Count { get; set; }
        public Classificator.TroopsEnum Type { get; set; }
    }
}
