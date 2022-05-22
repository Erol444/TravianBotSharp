using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using TbsCrossPlatform.Models.Database;

namespace TbsCrossPlatform.Services
{
    public class WebBrowser : IWebBrowser
    {
        private ChromeDriver _driver;

        private readonly ChromeDriverService _chromeService;
        private readonly HtmlDocument _html;

        public WebBrowser()
        {
            // Hide command prompt
            _chromeService = ChromeDriverService.CreateDefaultService();
            _chromeService.HideCommandPromptWindow = true;
            _html = new HtmlDocument();
        }

        public void Setup(Proxy proxy)
        {
            _driver = new(_chromeService);
        }

        public HtmlDocument GetHtml()
        {
            _html.LoadHtml(_driver.PageSource);
            return _html;
        }

        public ChromeDriver GetBrowser()
        {
            return _driver;
        }

        public void Close()
        {
            _driver.Quit();
            _driver = default;
        }

        public void Shutdown()
        {
            Close();
            _chromeService.Dispose();
        }
    }
}