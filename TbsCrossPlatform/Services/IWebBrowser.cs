using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using TbsCrossPlatform.Models.Database;

namespace TbsCrossPlatform.Services
{
    public interface IWebBrowser
    {
        public void Setup(Proxy proxy);

        public HtmlDocument GetHtml();

        public ChromeDriver GetBrowser();

        public void Close();

        public void Shutdown();
    }
}