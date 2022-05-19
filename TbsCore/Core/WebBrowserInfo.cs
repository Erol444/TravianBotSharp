using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using TbsCore.Database;
using TbsCore.Helpers;
using TbsCore.Helpers.ChromeExtension;

namespace TbsCore.Models.AccModels
{
    public class WebBrowserInfo : IDisposable
    {
        public ChromeDriver Driver { get; private set; }

        private readonly ChromeDriverService chromeService;

        private Account acc;
        public HtmlAgilityPack.HtmlDocument Html { get; private set; }

        public WebBrowserInfo()
        {
            // Hide command prompt
            chromeService = ChromeDriverService.CreateDefaultService();
            chromeService.HideCommandPromptWindow = true;
            Html = new HtmlAgilityPack.HtmlDocument();
        }

        public string CurrentUrl { get => Driver?.Url; }

        public async Task<bool> Init(Account acc, bool newAccess = true)
        {
            this.acc = acc;
            if (acc.Access.AllAccess.Count == 0)
            {
                acc.Logger.Warning("Account doesn't have any access. Press Edit account to check access");
                return false;
            }

            acc.Access.AllAccess.ForEach(a => a.Ok = true);

            Access.Access access = newAccess ? acc.Access.GetNewAccess() : acc.Access.GetCurrentAccess();

            if (!string.IsNullOrEmpty(access.Proxy))
            {
                var checkResult = await CheckProxy(acc);
                if (!checkResult) return false;
            }

            SetupChromeDriver(access, acc.AccInfo.Nickname, acc.AccInfo.ServerUrl);

            await Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
            return true;
        }

        private void SetupChromeDriver(Access.Access access, string username, string server)
        {
            ChromeOptions options = new ChromeOptions();
            if (!string.IsNullOrEmpty(access.Proxy))
            {
                options.AddExtension(DisableWebRTCLeak.GetPath());
                options.AddExtensions(FingerPrintDefender.GetPath());

                if (!string.IsNullOrEmpty(access.ProxyUsername))
                {
                    // Add proxy authentication
                    options.AddHttpProxy(access.Proxy, access.ProxyPort, access.ProxyUsername, access.ProxyPassword);
                }
                else
                {
                    options.AddArgument($"--proxy-server={access.Proxy}:{access.ProxyPort}");
                }
            }

            options.AddArgument($"--user-agent={access.Useragent}");

            // So websites (Travian) can't detect the bot
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
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
            var dir = IoHelperCore.UserCachePath(username, server, access.Proxy);
            Directory.CreateDirectory(dir);
            options.AddArguments("user-data-dir=" + dir);

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
                this.Driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(5);
            }
            catch (Exception e)
            {
                acc.Logger.Error(e, $"Error opening chrome driver! Is it already opened?");
            }
        }

        public ReadOnlyCollection<Cookie> GetCookies()
        {
            return Driver.Manage().Cookies.AllCookies;
        }

        /// <summary>
        /// Refresh page. Same as clicking F5
        /// </summary>
        public async Task<bool> Refresh() => await Navigate(CurrentUrl);

        public async Task<bool> Navigate(string url)
        {
            if (string.IsNullOrEmpty(url)) return await Refresh();

            try
            {
                // Will throw exception after timeout
                Driver.Navigate().GoToUrl(url);
            }
            catch (Exception e)
            {
                acc.Logger.Error(e, $"Error navigation to {url} - probably due to proxy/Internet or due to chrome still being opened");
                return false;
            }

            return await DriverHelper.WaitPageLoaded(acc);
        }

        public void UpdateHtml()
        {
            Html.LoadHtml(Driver.PageSource);
        }

        public void ExecuteScript(string script)
        {
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
            IJavaScriptExecutor js = acc.Wb.Driver;
            return (string)js.ExecuteScript("for(let field in Travian) { if (Travian[field].length == 32) return Travian[field]; }");
        }

        /// <summary>
        /// catch (WebDriverException e) when (e.Message.Contains("chrome not reachable") || e.Message.Contains("no such window:"))
        /// </summary>
        public bool CheckChromeOpen()
        {
            try
            {
                _ = Driver.Title;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckProxy(Account acc)
        {
            do
            {
                var currentAccess = acc.Access.GetCurrentAccess();
                if (string.IsNullOrEmpty(currentAccess.Proxy))
                {
                    return true;
                }

                if (!currentAccess.Ok)
                {
                    acc.Logger.Warning($"All proxies in your account is unusable! Please check your proxy status.");

                    return false;
                }

                var currentProxy = currentAccess.Proxy;
                acc.Logger.Information("Checking proxy " + currentProxy);

                var client = RestClientDatabase.Instance.GetRestClientIP(currentAccess);
                var result = await ProxyHelper.TestProxy(client, currentProxy);
                currentAccess.Ok = result;
                if (!result)
                {
                    // Proxy error!
                    acc.Logger.Warning($"Proxy {currentProxy} doesn't work! Trying different proxy");
                    if (acc.Access.AllAccess.Count > 1)
                    {
                        acc.Access.ChangeAccess();
                    }
                    else
                    {
                        acc.Logger.Warning($"There's only one proxy to this account! Please check your proxy status");
                        return false;
                    }
                }
                else
                {
                    acc.Logger.Information($"Proxy OK!");
                    return true;
                }
            }
            while (true);
        }

        public void Close()
        {
            if (Driver != null)
            {
                try
                {
                    var tabs = Driver.WindowHandles;
                    foreach (var tab in tabs)
                    {
                        Driver.SwitchTo().Window(tab);
                        Driver.Close();
                    }
                    Driver.Dispose();
                }
                catch { }

                Driver = null;
            }
        }

        public void Dispose()
        {
            Close();
            try
            {
                chromeService.Dispose();
            }
            catch { }
        }
    }
}