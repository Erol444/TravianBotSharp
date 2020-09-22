using TravBotSharp.Files.Models.ResourceModels;

namespace TravBotSharp.Files.Models.VillageModels
{
    public class VillRes
    {
        public void Init()
        {
            Capacity = new ResourceCapacity();
            Stored = new StoredResources();
            Stored.Init();
            Production = new Resources();
        }
        public Resources Production { get; set; }
        public ResourceCapacity Capacity { get; set; }
        public StoredResources Stored { get; set; }
        public long FreeCrop { get; set; }
    }
}
