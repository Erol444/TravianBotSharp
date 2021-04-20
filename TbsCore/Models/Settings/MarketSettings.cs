using TbsCore.Models.ResourceModels;

namespace TbsCore.Models.Settings
{
    public class MarketSettings
    {
        public SendResourcesConfiguration Configuration { get; set; }

        public void Init()
        {
            Configuration = new SendResourcesConfiguration();
            Configuration.Init();
        }
    }
}