using System;

namespace TravBotSharp.Files.Models.ResourceModels
{
    public class StoredResources
    {
        public void Init()
        {
            Resources = new Resources();
        }
        public Resources Resources { get; set; }
        public DateTime LastRefresh { get; set; }
    }
}
