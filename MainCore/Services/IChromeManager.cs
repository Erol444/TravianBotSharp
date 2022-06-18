namespace MainCore.Services
{
    public interface IChromeManager
    {
        public void LoadExtension();

        public ChromeBrowser Get(int id);

        public void Clear();
    }
}