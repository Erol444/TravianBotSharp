using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Files.Models.AccModels
{
    public class WebBrowserInfo
    {
        public ChromeDriver Driver { get; set; }
        public string CurrentUrl { get; set; }
        private Account acc;
        public HtmlAgilityPack.HtmlDocument Html { get; set; }

        public void InitSelenium(Account acc)
        {
            this.acc = acc;
            var access = acc.Access.GetNewAccess();

            SetupChromeDriver(access, acc.AccInfo.Nickname, acc.AccInfo.ServerUrl);

            if(this.Html == null)
            {
                this.Html = new HtmlAgilityPack.HtmlDocument();
            }

            // If account is using a proxy, check if the proxy is working correctly
            if (!string.IsNullOrEmpty(access.Proxy))
            {
                var checkProxy = new CheckProxy()
                {
                    ExecuteAt = DateTime.MinValue.AddMinutes(1)
                };
                TaskExecutor.AddTask(acc, checkProxy);
            }
        }

        private void SetupChromeDriver(Access access, string username, string server)
        {
            ChromeOptions options = new ChromeOptions();
            if (!string.IsNullOrEmpty(access.Proxy))
            {
                options.AddArgument($"--proxy-server={access.Proxy}:{access.ProxyPort}");
                options.AddArgument("ignore-certificate-errors");
            }
            if (!string.IsNullOrEmpty(access.UserAgent))
            {
                options.AddArgument("--user-agent=" + access.UserAgent);
            }

            // Make browser headless to preserve memory resources
            if(acc.Settings.HeadlessMode) options.AddArguments("headless");

            // Do not download images in order to preserve memory resources
            if (acc.Settings.DisableImages) options.AddArguments("--blink-settings=imagesEnabled=false");

            // Add browser caching
            var accFolder = IoHelperCore.GetCacheFolder(username, server, access.Proxy);
            var dir = Path.Combine(IoHelperCore.CachePath, accFolder);
            Directory.CreateDirectory(dir);
            options.AddArguments("user-data-dir=" + dir);

            // Disable message "Chrome is being controlled by automated test software"
            //options.AddArgument("--excludeSwitches=enable-automation"); // Doesn't work anymore

            // Hide command prompt
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            try
            {
                this.Driver = new ChromeDriver(service, options);
            }
            catch(Exception e)
            {
                // TODO: log exception
            }
        }

        internal Dictionary<string, string> GetCookes()
        {
            var cookies = Driver.Manage().Cookies.AllCookies;
            var cookiesDir = new Dictionary<string, string>();
            for (int i = 0; i < cookies.Count; i++)
            {
                cookiesDir.Add(cookies[i].Name, cookies[i].Value);
            }
            return cookiesDir;
        }

        public async Task Navigate(string url)
        {
            this.Driver.Navigate().GoToUrl(url);
            this.CurrentUrl = url;
            await Task.Delay(AccountHelper.Delay());
            this.Html.LoadHtml(this.Driver.PageSource);
            await TaskExecutor.PageLoaded(acc);
        }

        public void Close()
        {
            try
            {
                this.Driver.Close();
                this.Driver.Dispose();
            }
            catch(Exception e)
            {
                Utils.log.Warning($"Exception occured when trying to close selenium driver. Acc {acc.AccInfo.Nickname}\n{e.Message}");
            }
        }
    }
}