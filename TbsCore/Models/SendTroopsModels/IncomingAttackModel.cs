using TravBotSharp.Files.Models.SendTroopsModels;

namespace TravBotSharp.Files.Models.AttackModels
{
    public class IncomingAttackModel : TroopsMovementModel
    {
        /// <summary>
        /// Can be false just in case user has spies art or if number of troops in attack is below
        /// rally point level
        /// </summary>
        public bool Hero { get; set; }
    }

}
