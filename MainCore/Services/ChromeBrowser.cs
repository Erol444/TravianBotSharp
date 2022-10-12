using HtmlAgilityPack;
using MainCore.Models.Database;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;

namespace MainCore.Services
{
    public sealed class ChromeBrowser : IChromeBrowser
    {
        private ChromeDriver _driver;
        private readonly ChromeDriverService _chromeService;
        private WebDriverWait _wait;

        private readonly string[] _extensionsPath;
        private readonly HtmlDocument _htmlDoc = new();

        public ChromeBrowser(string[] extensionsPath)
        {
            _extensionsPath = extensionsPath;

            _chromeService = ChromeDriverService.CreateDefaultService();
            _chromeService.HideCommandPromptWindow = true;
        }

        public void Setup(Access access, AccountSetting setting)
        {
            ChromeOptions options = new();

            options.AddExtensions(_extensionsPath);

            if (!string.IsNullOrEmpty(access.ProxyHost))
            {
                if (!string.IsNullOrEmpty(access.ProxyUsername))
                {
                    options.AddHttpProxy(access.ProxyHost, access.ProxyPort, access.ProxyUsername, access.ProxyPassword);
                }
                else
                {
                    options.AddArgument($"--proxy-server={access.ProxyHost}:{access.ProxyPort}");
                }
            }

            options.AddArgument($"--user-agent={access.Useragent}");

            // So websites (Travian) can't detect the bot
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-features=UserAgentClientHint");
            options.AddArgument("--disable-logging");
            options.AddArgument("--no-sandbox");

            options.AddArgument("--mute-audio");
            if (setting.IsDontLoadImage) options.AddArguments("--blink-settings=imagesEnabled=false"); //--disable-images

            var path = Path.Combine(AppContext.BaseDirectory, "Data", "Cache", access.ProxyHost ?? "default");
            Directory.CreateDirectory(path);
            options.AddArguments($"user-data-dir={path}");

            _driver = new ChromeDriver(_chromeService, options);
            if (setting.IsMinimized) _driver.Manage().Window.Minimize();

            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(1);
            _wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(3));
        }

        public ChromeDriver GetChrome() => _driver;

        public HtmlDocument GetHtml()
        {
            UpdateHtml();
            return _htmlDoc;
        }

        public WebDriverWait GetWait() => _wait;

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

        public bool IsOpen()
        {
            try
            {
                _ = _driver.Title;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetCurrentUrl() => _driver.Url;

        public void Navigate(string url = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                Navigate(GetCurrentUrl());
                return;
            }

            _driver.Navigate().GoToUrl(url);
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        private void UpdateHtml(string source = null)
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