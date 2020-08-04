using TravBotSharp.Files.Tasks.ResourcesConfiguration;

namespace TravBotSharp.Files.Models.Settings
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
