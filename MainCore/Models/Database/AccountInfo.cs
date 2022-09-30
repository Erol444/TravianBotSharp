using MainCore.Enums;

namespace MainCore.Models.Database
{
    public class AccountInfo
    {
        public int AccountId { get; set; }
        public TribeEnums Tribe { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public bool HasPlusAccount { get; set; }
    }
}