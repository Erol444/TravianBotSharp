using TbsCore.Models.ResourceModels;

namespace TbsCore.Models.VillageModels
{
    public class VillRes
    {
        public Resources Production { get; set; }
        public ResourceCapacity Capacity { get; set; }
        public StoredResources Stored { get; set; }
        public long FreeCrop { get; set; }

        public void Init()
        {
            Capacity = new ResourceCapacity();
            Stored = new StoredResources();
            Stored.Init();
            Production = new Resources();
        }
    }
}