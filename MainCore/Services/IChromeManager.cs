using System;

namespace MainCore.Services
{
    public interface IChromeManager : IDisposable
    {
        public void LoadExtension();

        public IChromeBrowser Get(int id);
    }
}