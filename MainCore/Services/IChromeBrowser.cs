using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MainCore.Services
{
    public interface IChromeBrowser
    {
        public void Setup();

        public void Stop();

        public void Close();

        public void Shutdown();

        public string GetCurrentUrl();

        public ChromeDriver GetChrome();

        public HtmlDocument GetHtml();

        public WebDriverWait GetWait();

        public void Navigate(string url);

        public void UpdateHtml(string source);

        public void WaitPageLoaded();
    }
}