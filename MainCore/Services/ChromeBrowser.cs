using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MainCore.Services
{
    public class ChromeBrowser : IChromeBrowser
    {
        private ChromeDriver _driver;
        private readonly ChromeDriverService _chromeService;
        private WebDriverWait _wait;

        private readonly string[] _extensionsPath;
        private readonly HtmlDocument _htmlDoc = new();
        private long _isStop;

        private bool IsStop
        {
            get
            {
                return Interlocked.Read(ref _isStop) == 1;
            }
            set
            {
                Interlocked.Exchange(ref _isStop, Convert.ToInt64(value));
            }
        }

        public ChromeBrowser(ChromeDriverService chromeService, string[] extensionsPath)
        {
            _extensionsPath = extensionsPath;
            _chromeService = chromeService;
        }

        public void Setup()
        {
            ChromeOptions options = new();

            options.AddExtensions(_extensionsPath);

            //if (proxy.Host is not null)
            //{
            //    options.AddHttpProxy(proxy.Host, int.Parse(proxy.Port), proxy.Username, proxy.Password);
            //}
            // So websites (Travian) can't detect the bot
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-features=UserAgentClientHint");
            options.AddArgument("--disable-logging");
            //if (!string.IsNullOrEmpty(_useragent))
            //{
            //    options.AddArgument($"--user-agent={_useragent}");
            //}
            // Mute audio because of the Ads
            options.AddArgument("--mute-audio");
            options.AddArgument("--no-sandbox");

            _driver = new ChromeDriver(_chromeService, options);
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(1);
            _wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(1));

            Navigate("https://www.travian.com/");
            IsStop = false;
        }

        public ChromeDriver GetChrome() => _driver;

        public HtmlDocument GetHtml() => _htmlDoc;

        public WebDriverWait GetWait() => _wait;

        public void Stop()
        {
            IsStop = true;
        }

        public void Shutdown()
        {
            Close();
            _chromeService.Dispose();
        }

        public void Close()
        {
            if (_driver is null) return;

            try
            {
                _driver.Quit();
            }
            catch { }
            _driver = null;
        }

        public string GetCurrentUrl() => _driver.Url;

        public void Navigate(string url = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            _driver.Navigate().GoToUrl(url);
        }

        public void UpdateHtml(string source = null)
        {
            if (string.IsNullOrEmpty(source))
            {
                _htmlDoc.LoadHtml(_driver.PageSource);
            }
            else
            {
                _htmlDoc.LoadHtml(source);
            }
        }

        public void WaitPageLoaded()
        {
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}