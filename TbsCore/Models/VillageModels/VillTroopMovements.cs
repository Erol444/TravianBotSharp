using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.TroopsMovementModels;

namespace TbsCore.Models.VillageModels
{
    public class VillTroopMovements
    {
        public void Init() 
        {
            IncomingAttacks = new List<TroopsMovementModel>();
        }

        /// <summary>
        /// Incoming attacks
        /// </summary>
        public List<TroopsMovementModel> IncomingAttacks { get; set; }

        /// <summary>
        /// Troops movement parsed from dorf1
        /// </summary>
        public List<TroopMovementDorf1> Dorf1Movements { get; set; }
    }
}
