namespace MainCore.Services.Interface
{
    public interface IChromeManager
    {
        public void LoadExtension();

        public IChromeBrowser Get(int id);

        public void Shutdown();
    }
}