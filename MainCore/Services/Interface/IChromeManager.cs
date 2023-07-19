namespace MainCore.Services.Interface
{
    public interface IChromeManager
    {
        public void LoadExtension();

        public IChromeBrowser Get(int accountId);

        public void Shutdown();
        void LoadDriver();
    }
}