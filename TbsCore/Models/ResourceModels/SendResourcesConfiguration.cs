namespace TbsCore.Models.ResourceModels
{
    public class SendResourcesConfiguration
    {
        public void Init()
        {
            Enabled = false;
            Condition = new Resources();
            Amount = new Resources();
        }

        public bool Enabled { get; set; }
        public Resources Condition { get; set; }
        public Resources Amount { get; set; }
    }
}