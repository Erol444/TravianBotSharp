using TbsCore.Models.ResourceModels;

namespace TbsCore.Models.JsObjects
{
    public class ResourcesJsObject
    {
        public Production production { get; set; }
        public Storage storage { get; set; }
        public MaxStorage maxStorage { get; set; }
    }

    public class Production
    {
        public long l1 { get; set; }
        public long l2 { get; set; }
        public long l3 { get; set; }
        public long l4 { get; set; }
        public long l5 { get; set; }

        public Resources GetResources()
        {
            return new Resources()
            {
                Wood = this.l1,
                Clay = this.l2,
                Iron = this.l3,
                Crop = this.l4,
            };
        }
    }

    public class Storage
    {
        public long l1 { get; set; }
        public long l2 { get; set; }
        public long l3 { get; set; }
        public long l4 { get; set; }

        public Resources GetResources()
        {
            return new Resources()
            {
                Wood = this.l1,
                Clay = this.l2,
                Iron = this.l3,
                Crop = this.l4,
            };
        }
    }

    public class MaxStorage
    {
        public long l1 { get; set; }
        public long l2 { get; set; }
        public long l3 { get; set; }
        public long l4 { get; set; }
    }
}
