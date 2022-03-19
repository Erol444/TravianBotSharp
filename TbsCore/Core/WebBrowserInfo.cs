﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models;
using TbsCore.Tasks.LowLevel;
using TbsCore.Helpers.Extension;
using static TbsCore.Tasks.BotTask;

namespace TbsCore.Models.AccModels
{
    public class WebBrowserInfo : IDisposable
    {
        public ChromeDriver Driver { get; private set; }

        private ChromeDriverService chromeService;

        public string CurrentUrl
        {
            get
            {
                CheckChromeOpen();
                return this.Driver.Url;
            }
        }

        private Account acc;
        public HtmlAgilityPack.HtmlDocument Html { get; set; }

        /// <summary>
        /// Http client, configured with proxy
        /// </summary>
        public RestClient RestClient { get; set; }

        public async Task InitSelenium(Account acc, bool newAccess = true)
        {
            this.acc = acc;
            Access.Access access = newAccess ? acc.Access.GetNewAccess() : acc.Access.GetCurrentAccess();

            SetupChromeDriver(access, acc.AccInfo.Nickname, acc.AccInfo.ServerUrl);

            if (this.Html == null)
            {
                this.Html = new HtmlAgilityPack.HtmlDocument();
            }

            InitHttpClient(access);
            if (!string.IsNullOrEmpty(access.Proxy))
            {
                var checkproxy = new CheckProxy();
                await checkproxy.Execute(acc);
            }
            else await this.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
        }

        private void InitHttpClient(Access.Access a)
        {
            RestClient = new RestClient();
            HttpHelper.InitRestClient(a, RestClient);
        }

        private void SetupChromeDriver(Access.Access access, string username, string server)
        {
            ChromeOptions options = new ChromeOptions();

            // Turn on logging preferences for buildings localization (string).
            //var loggingPreferences = new OpenQA.Selenium.Chromium.ChromiumPerformanceLoggingPreferences();
            //loggingPreferences.IsCollectingNetworkEvents = true;
            //options.PerformanceLoggingPreferences = loggingPreferences;
            //options.SetLoggingPreference("performance", LogLevel.All);

            if (!string.IsNullOrEmpty(access.Proxy))
            {
                // add WebRTC Leak
                var extensionPath = DisableWebRTCLeak.CreateExtension(username, server, access);
                options.AddExtension(extensionPath);

                if (!string.IsNullOrEmpty(access.ProxyUsername))
                {
                    // Add proxy authentication
                    extensionPath = ProxyAuthentication.CreateExtension(username, server, access);
                    options.AddExtension(extensionPath);
                }

                options.AddArgument($"--proxy-server={access.Proxy}:{access.ProxyPort}");
                options.AddArgument("ignore-certificate-errors"); // --ignore-certificate-errors ?
            }

            //options.AddArgument($"--user-agent={access.UserAgent}");

            //options.AddArguments("--disable-logging");
            //options.AddArguments("--disable-metrics");
            //options.AddArguments("--disable-dev-tools");
            //options.AddArguments("--disable-gpu-shader-disk-cache");
            //options.AddArguments("--aggressive-cache-discard");
            //options.AddArguments("--arc-disable-gms-core-cache");

            // So websites (Travian) can't detect the bot
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-features=UserAgentClientHint");
            options.AddArguments("--disable-logging");
            options.AddArguments("--disable-infobars");

            //options.AddAdditionalCapability("deviceOrientation", "landscape", true);
            //options.AddAdditionalCapability("resolution", "1280x720", true);

            // Mute audio because of the Ads
            options.AddArguments("--mute-audio");

            // Make browser headless to preserve memory resources
            // if (acc.Settings.HeadlessMode) options.AddArguments("headless");

            // Do not download images in order to preserve memory resources / proxy traffic
            if (acc.Settings.DisableImages) options.AddArguments("--blink-settings=imagesEnabled=false"); //--disable-images

            // Add browser caching
            var dir = IoHelperCore.GetCacheDir(username, server, access);
            Directory.CreateDirectory(dir);
            options.AddArguments("user-data-dir=" + dir);

            // Hide command prompt
            chromeService = ChromeDriverService.CreateDefaultService();
            chromeService.HideCommandPromptWindow = true;

            try
            {
                if (acc.Settings.OpenMinimized)
                {
                    options.AddArguments("--window-position=5000,5000");
                    this.Driver = new ChromeDriver(chromeService, options);
                    this.Driver.Manage().Window.Position = new System.Drawing.Point(200, 200); // TODO: change coords?
                    this.Driver.Manage().Window.Minimize();
                }
                else this.Driver = new ChromeDriver(chromeService, options);

                // Set timeout
                this.Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            }
            catch (Exception e)
            {
                acc.Logger.Error(e, $"Error opening chrome driver! Is it already opened?");
            }
        }

        internal Dictionary<string, string> GetCookies()
        {
            var cookies = Driver.Manage().Cookies.AllCookies;
            var cookiesDir = new Dictionary<string, string>();
            for (int i = 0; i < cookies.Count; i++)
            {
                cookiesDir.Add(cookies[i].Name, cookies[i].Value);
            }
            return cookiesDir;
        }

        /// <summary>
        /// Refresh page. Same as clicking F5
        /// </summary>
        public async Task Refresh() => await Navigate(this.CurrentUrl);

        public async Task Navigate(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            int repeatCnt = 0;
            bool repeat;
            do
            {
                CheckChromeOpen();

                try
                {
                    // Will throw exception after timeout
                    this.Driver.Navigate().GoToUrl(url);
                    repeat = false;
                }
                catch (Exception e)
                {
                    acc.Logger.Error(e, $"Error navigation to {url} - probably due to proxy/Internet or due to chrome still being opened");
                    repeat = true;
                    if (5 <= ++repeatCnt && !string.IsNullOrEmpty(acc.Access.GetCurrentAccess().Proxy))
                    {
                        // Change access
                        repeatCnt = 0;
                        var changeAccess = new ChangeAccess();
                        await changeAccess.Execute(acc);
                        await Task.Delay(AccountHelper.Delay(acc) * 5);
                    }
                    await Task.Delay(AccountHelper.Delay(acc));
                }
            }
            while (repeat);

            await Task.Delay(AccountHelper.Delay(acc));
            UpdateHtml();
            await TaskExecutor.PageLoaded(acc);
        }

        public void UpdateHtml()
        {
            CheckChromeOpen();
            Html.LoadHtml(Driver.PageSource);
        }

        public void ExecuteScript(string script)
        {
            CheckChromeOpen();
            Driver.ExecuteScript(script);
        }

        /// <summary>
        /// Gets JS object from the game. Query examples:
        /// window.TravianDefaults.Map.Size.top
        /// resources.maxStorage
        /// Travian.Game.speed
        /// </summary>
        /// <param name="obj">JS object</param>
        /// <returns>Long for number, bool for boolean, string otherwise</returns>
        public T GetJsObj<T>(string obj)
        {
            IJavaScriptExecutor js = acc.Wb.Driver;
            return (T)js.ExecuteScript($"return {obj};");
        }

        /// <summary>
        /// Get bearer token for Travian T4.5
        /// </summary>
        public string GetBearerToken()
        {
            CheckChromeOpen();
            IJavaScriptExecutor js = acc.Wb.Driver;
            return (string)js.ExecuteScript("return Travian.abrogatedReanimatingAftereffects");
        }

        public IWebElement FindElementById(string element)
        {
            CheckChromeOpen();
            return Driver.FindElementById(element);
        }

        public IWebElement FindElementByXPath(string xPath)
        {
            CheckChromeOpen();
            return Driver.FindElementByXPath(xPath);
        }

        public ITargetLocator SwitchTo()
        {
            CheckChromeOpen();
            return Driver.SwitchTo();
        }

        /// <summary>
        /// Throw WebDriverException when Chrome closed or not responding
        /// </summary>
        public void CheckChromeOpen()
        {
            _ = Driver.Title;
        }

        public void Dispose()
        {
            while (Driver != default)
            {
                try
                {
                    Driver.Close();
                    Driver.Quit(); // Also disposes
                    Driver = default;
                }
                catch (WebDriverException)
                {
                    Driver.Quit();
                    Driver = default;
                }
                catch (Exception)
                {
                }
            }

            chromeService.Dispose();
        }
    }
}