using TbsCore.Models.ResourceModels;

namespace TbsCore.Models.Settings
{
    public class MarketSettings
    {
        public void Init()
        {
            Configuration = new SendResourcesConfiguration();
            Configuration.Init();
        }
        public SendResourcesConfiguration Configuration { get; set; }
    }
}
