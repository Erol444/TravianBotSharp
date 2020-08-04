using TravBotSharp.Files.Models.SendTroopsModels;

namespace TravBotSharp.Files.Models.AttackModels
{
    public class SendWaveModel : TroopsMovementModel
    {
        /// <summary>
        /// Number of milliseconds to be waited before sending another attack.
        /// If delay is set, Arrival will be ignored.
        /// </summary>
        public int DelayMs { get; set; }
        /// <summary>
        /// Dirty hack. Will send all off that is in the village.
        /// TODO: after bot has values of troops in village, remove this.
        /// </summary>
        public bool AllOff { get; set; }
        /// <summary>
        /// Dirty hack like one above. 19 troops + catapult
        /// </summary>
        public bool FakeAttack { get; set; }

    }

}
