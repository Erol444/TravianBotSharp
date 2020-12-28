namespace TbsCore.Models.SendTroopsModels
{
    public class SendDeffAmount
    {
        /// <summary>
        /// Amount of deff to be sent to a village. If null, send all deff.
        /// </summary>
        public int? DeffCount { get; set; }
    }
}
