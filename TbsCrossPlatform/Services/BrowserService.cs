using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TbsCrossPlatform.Models.Database;

namespace TbsCrossPlatform.Services
{
    public class BrowserService : IBrowserService
    {
        private readonly Dictionary<string, IWebBrowser> dictionary = new();

        public IWebBrowser GetBrowser(string id, Proxy proxy)
        {
            if (!dictionary.TryGetValue($"{id}{proxy.Host}", out IWebBrowser browser)) return null;
            return browser;
        }

        public void Setup(string id, Proxy proxy)
        {
            var key = $"{id}{proxy.Host}";
            if (dictionary.ContainsKey(key)) return;
            IWebBrowser browser = new WebBrowser();
            browser.Setup(proxy);
            dictionary.Add(key, browser);
        }
    }
}