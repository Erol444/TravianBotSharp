namespace MainCore.Models.Runtime
{
    public class VillageInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool UnderAttack { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}