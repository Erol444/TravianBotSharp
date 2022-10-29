using System;

namespace MainCore.Services.Interface
{
    public interface IChromeManager : IDisposable
    {
        public void LoadExtension();

        public IChromeBrowser Get(int id);
    }
}