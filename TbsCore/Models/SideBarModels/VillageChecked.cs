namespace TravBotSharp.Files.Models
{
    public class VillageChecked
    {
        public string Name { get; set; }
        public bool UnderAttack { get; set; }
        public Coordinates Coordinates { get; set; }
        public int Id { get; set; }
        public bool Active { get; set; }
    }
}
