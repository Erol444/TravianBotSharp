using System.IO;
using System.IO.Compression;
using TbsCore.Models.Access;

namespace TbsCore.Helpers.Extension
{
    public static class DisableWebRTCLeak
    {
        /// <summary>
        /// Manifest.json string for the chrome extension
        /// </summary>
        private static readonly string manifestJson = "{\"name\":\"WebRTC Protect - Protect IP Leak\",\"description\":\"__MSG_description__\",\"version\":\"0.1.7\",\"manifest_version\":2,\"permissions\":[\"storage\",\"privacy\",\"contextMenus\"],\"background\":{\"persistent\":false,\"scripts\":[\"common.js\"]}}";

        private static readonly string backgroundJs = "\"use strict\";function action(){chrome.storage.local.get({enabled:!0,eMode:/Firefox/.test(navigator.userAgent)?\"proxy_only\":\"disable_non_proxied_udp\",dMode:\"default_public_interface_only\"},e=>{const o=e.enabled?e.eMode:e.dMode;chrome.privacy.network.webRTCIPHandlingPolicy.clear({},()=>{chrome.privacy.network.webRTCIPHandlingPolicy.set({value:o},()=>{chrome.privacy.network.webRTCIPHandlingPolicy.get({},n=>{})})})})}action(),chrome.storage.onChanged.addListener(()=>{action()}),chrome.browserAction.onClicked.addListener(()=>{chrome.storage.local.get({enabled:!0},e=>chrome.storage.local.set({enabled:!e.enabled}))});{const e=()=>chrome.contextMenus.create({id:\"leakage\",contexts:[\"browser_action\"],title:\"Check WebTRC Leakage\"});chrome.runtime.onInstalled.addListener(e),chrome.runtime.onStartup.addListener(e)}chrome.contextMenus.onClicked.addListener(()=>{chrome.tabs.create({url:\"https://webbrowsertools.com/ip-address/\"})});";

        /// <summary>

        /// Creates chrome extension (.crx) for proxy authentication
        /// </summary>
        /// <param name="username">Travian username</param>
        /// <param name="server">Travian server</param>
        /// <param name="access">Access</param>
        /// <returns>Path of the chrome extension</returns>
        public static string CreateExtension(string username, string server, Access access)
        {
            var cacheDir = IoHelperCore.UserCachePath(username, server, access.Proxy);
            var dir = Path.Combine(cacheDir, "DisableWebRTCLeak");
            Directory.CreateDirectory(dir);

            CreateFile(Path.Combine(dir, "manifest.json"), manifestJson);
            CreateFile(Path.Combine(dir, "common.js"), backgroundJs);

            var zipPath = Path.Combine(cacheDir, "DisableWebRTCLeak.crx");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(dir, zipPath);

            return zipPath;
        }

        /// <summary>
        /// Create a text file and write to it
        /// </summary>
        /// <param name="path">Path where to create the file</param>
        /// <param name="text">Text to write to the file</param>
        private static void CreateFile(string path, string text)
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.Write(text);
            }
        }
    }
}