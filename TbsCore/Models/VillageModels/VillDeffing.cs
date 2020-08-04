namespace TravBotSharp.Files.Models.VillageModels
{
    public class VillDeffing
    {
        /// <summary>
        /// In which cases do we alert the user
        /// </summary>
        public AlertTypeEnum AlertType { get; set; }
        /// <summary>
        /// If player has better spies artifact, alert only if there is hero in the attack.
        /// If no spies art, this gets ignores.
        /// </summary>
        public bool AlertOnHero { get; set; }

        // TODO: Add: Deff type: Full deff, cut waves, remove troops etc.

    }
    public enum AlertTypeEnum
    {
        Disabled,
        FullAttack,
        AnyAttack
    }
}