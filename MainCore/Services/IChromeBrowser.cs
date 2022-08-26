using HtmlAgilityPack;
using MainCore.Models.Database;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MainCore.Services
{
    public interface IChromeBrowser
    {
        public void Setup(Access access, AccountSetting setting);

        public void Close();

        public void Shutdown();

        public string GetCurrentUrl();

        public bool IsOpen();

        public ChromeDriver GetChrome();

        public HtmlDocument GetHtml();

        public WebDriverWait GetWait();

        public void Navigate(string url = null);

        public void WaitPageLoaded();
    }
}