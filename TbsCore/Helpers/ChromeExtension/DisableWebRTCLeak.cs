using System;
using System.IO;
using System.IO.Compression;
using TbsCore.Models.Access;

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