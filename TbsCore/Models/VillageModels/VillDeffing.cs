using System.Collections.Generic;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.VillageModels;

namespace TbsCore.Models.VillageModels
{
    public class VillDeffing
    {
        public void Init()
        {
        }

        /// <summary>
        /// In which cases do we alert the user
        /// </summary>
        public AlertTypeEnum AlertType { get; set; }

        /// <summary>
        /// If player has better spies artifact, alert only if there is hero in the attack.
        /// If no spies art, this gets ignored (since there will be "?" below hero anyways)
        /// </summary>
        public bool OnlyAlertOnHero { get; set; }

        // TODO: Add: Deff type: Full deff, cut waves, hide troops etc.
    }
}

namespace TbsCore.Models.VillageModels
{
    public enum AlertTypeEnum
    {
        Disabled,
        FullAttack,
        AnyAttack
    }
}