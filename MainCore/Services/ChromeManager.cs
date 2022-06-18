using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MainCore.Services
{
    public class ChromeManager : IChromeManager
    {
        private ConcurrentDictionary<int, ChromeBrowser> _dictionary = new();
        private readonly ChromeDriverService _chromeService;

        private string[] _extensionsPath;

        public ChromeManager()
        {
            _chromeService = ChromeDriverService.CreateDefaultService();
            _chromeService.HideCommandPromptWindow = true;
            _chromeService.Start();
        }
        public ChromeBrowser Get(int id)
        {
            var result = _dictionary.TryGetValue(id, out ChromeBrowser browser);
            if (result) return browser;

            browser = new ChromeBrowser(_chromeService, _extensionsPath);
            _dictionary.TryAdd(id, new ChromeBrowser(_chromeService, _extensionsPath));

            return browser;
        }

        public void Clear()
        {
            foreach (var id in _dictionary.Keys)
            {
                _dictionary.Remove(id, out ChromeBrowser browser);
                browser.Shutdown();
            }
            _chromeService.Dispose();
        }

        public void LoadExtension()
        {
            var extenstionDir = Path.Combine(AppContext.BaseDirectory, "ExtensionFile");
            var isCreated = false;
            if (Directory.Exists(extenstionDir)) isCreated = true;
            else Directory.CreateDirectory(extenstionDir);

            var asmb = Assembly.GetExecutingAssembly();
            var extensionsName = asmb.GetManifestResourceNames();
            var list = new List<string>();

            for (var i = 1; i < extensionsName.Length; i++)
            {
                var extensionName = extensionsName[i];
                var path = Path.Combine(extenstionDir, extensionName);
                list.Add(path);
                if (!isCreated)
                {
                    using Stream input = asmb.GetManifestResourceStream(extensionName);
                    using Stream output = File.Create(path);
                    input.CopyTo(output);
                }
            }

            _extensionsPath = list.ToArray();
        }
    }
}