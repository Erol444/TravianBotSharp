using MainCore.Enums;

namespace MainCore.Models.Database
{
    public class Hero
    {
        public int AccountId { get; set; }
        public int Health { get; set; }
        public HeroStatusEnums Status { get; set; }
    }
}