using System.Collections.Generic;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.TroopsMovementModels;

namespace TbsCore.Models.VillageModels
{
    public class VillTroopMovements
    {
        public void Init()
        {
            IncomingAttacks = new List<TroopsMovementRallyPoint>();
        }

        /// <summary>
        /// Incoming attacks
        /// </summary>
        public List<TroopsMovementRallyPoint> IncomingAttacks { get; set; }

        /// <summary>
        /// Troops movement parsed from dorf1
        /// </summary>
        public List<TroopMovementDorf1> Dorf1Movements { get; set; }
    }
}
