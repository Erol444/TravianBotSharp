using TbsCore.Models.ResourceModels;

namespace TbsCore.Models.Settings
{
    public class AutoMarketSettings
    {
        public void Init()
        {
            SendToMain = new SendResourcesConfiguration();
            SendToMain.Init();
            SendToNeed = new SendResourcesConfiguration();
            SendToNeed.Init();
        }

        public SendResourcesConfiguration SendToMain { get; set; }
        public SendResourcesConfiguration SendToNeed { get; set; }
        public bool NeedWhenBuild { get; set; }
        public bool NeedWhenTrain { get; set; }
        public bool NeedWhenOther { get; set; }
    }
}