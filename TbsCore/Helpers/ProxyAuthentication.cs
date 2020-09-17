using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TbsCore.Helpers
{
    public class ProxyAuthentication
    {
        /// <summary>
        /// Manifest.json string for the chrome extension
        /// </summary>
        private readonly string manifestJson = "{\"version\": \"1.0.0\",\"manifest_version\": 2,\"name\": \"TBS Proxy Auth Extension\",\"permissions\": [\"proxy\",\"tabs\",\"unlimitedStorage\",\"storage\",\"<all_urls>\",\"webRequest\",\"webRequestBlocking\"],\"background\": {\"scripts\": [\"background.js\"]},\"minimum_chrome_version\":\"22.0.0\"}";

        /// <summary>
        /// Creates chrome extension (.crx) for proxy authentication
        /// </summary>
        /// <param name="username">Travian username</param>
        /// <param name="server">Travian server</param>
        /// <param name="access">Access</param>
        /// <returns>Path of the chrome extension</returns>
        public string CreateExtension(string username, string server, Access access)
        {
            var cacheDir = IoHelperCore.GetCacheDir(username, server, access);
            var dir = Path.Combine(cacheDir, "ProxyAuthExtension");
            Directory.CreateDirectory(dir);

            CreateFile(Path.Combine(dir, "manifest.json"), manifestJson);
            CreateFile(Path.Combine(dir, "background.js"), GenerateBackgroundJs(access));

            var zipPath = Path.Combine(cacheDir, "chromeExtension.crx");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(dir, zipPath);

            return zipPath;
        }

        /// <summary>
        /// Generates string for the background.js file of the chrome extension
        /// </summary>
        /// <param name="access">Extension</param>
        /// <returns>JS code for the extension</returns>
        private string GenerateBackgroundJs(Access access)
        {
            return @"
var config = {
    mode: 'fixed_servers',
    rules: {
        singleProxy: {
            scheme: 'http',
            host: '" + access.Proxy.Trim() +@"',
            port: "+ access.ProxyPort +@"
        },
        bypassList:['localhost']
    }
};
chrome.proxy.settings.set({value: config, scope: 'regular'}, function() { });

function callbackFn(details)
{
    return { authCredentials: { username: '" + access.ProxyUsername.Trim() + @"', password: '" + access.ProxyPassword.Trim() + @"' } };
}

chrome.webRequest.onAuthRequired.addListener(
    callbackFn,
    {urls: ['<all_urls>']},
    ['blocking']
);
";
        }

        /// <summary>
        /// Create a text file and write to it
        /// </summary>
        /// <param name="path">Path where to create the file</param>
        /// <param name="text">Text to write to the file</param>
        private void CreateFile(string path, string text)
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.Write(text);
            }
        }
    }
}
