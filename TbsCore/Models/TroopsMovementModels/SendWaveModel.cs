namespace TbsCore.Models.SendTroopsModels
{
    public class SendWaveModel : TroopsSendModel
    {
        /// <summary>
        ///     Number of milliseconds to be waited before sending another wave.
        ///     If delay is set, Arrival will be ignored for all proceeding waves
        /// </summary>
        public int DelayMs { get; set; }

        /// <summary>
        ///     Dirty hack like one above. 19 troops + catapult
        /// </summary>
        public bool FakeAttack { get; set; }
    }
}