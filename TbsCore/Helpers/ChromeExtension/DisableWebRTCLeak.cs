using System;
using System.IO;

namespace TbsCore.Helpers.ChromeExtension
{
    public static class DisableWebRTCLeak
    {
        public static string GetPath()
        {
            var extenstionDir = Path.Combine(AppContext.BaseDirectory, "Assets", "ChromeExtension");
            var extenstionPath = Path.Combine(extenstionDir, "DisableWebRTCLeak.crx");

            return extenstionPath;
        }
    }
}