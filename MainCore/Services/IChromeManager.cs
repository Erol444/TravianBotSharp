namespace MainCore.Services
{
    public interface IChromeManager
    {
        public void LoadExtension();

        public IChromeBrowser Get(int id);

        public void Clear();
    }
}