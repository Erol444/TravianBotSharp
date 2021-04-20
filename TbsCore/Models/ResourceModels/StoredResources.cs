using System;

namespace TbsCore.Models.ResourceModels
{
    public class StoredResources
    {
        public Resources Resources { get; set; }
        public DateTime LastRefresh { get; set; }

        public void Init()
        {
            Resources = new Resources();
        }
    }
}