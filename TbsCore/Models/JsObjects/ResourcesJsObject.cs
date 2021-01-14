using System;
using System.Collections.Generic;
using System.Text;
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
        public int l1 { get; set; }
        public int l2 { get; set; }
        public int l3 { get; set; }
        public int l4 { get; set; }
        public int l5 { get; set; }

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
        public int l1 { get; set; }
        public int l2 { get; set; }
        public int l3 { get; set; }
        public int l4 { get; set; }

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
        public int l1 { get; set; }
        public int l2 { get; set; }
        public int l3 { get; set; }
        public int l4 { get; set; }
    }
}
