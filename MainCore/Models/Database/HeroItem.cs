using MainCore.Enums;

namespace MainCore.Models.Database
{
    public class HeroItem
    {
        public int AccountId { get; set; }
        public int Id { get; set; }
        public HeroItemEnums Item { get; set; }
        public int Count { get; set; }
    }
}