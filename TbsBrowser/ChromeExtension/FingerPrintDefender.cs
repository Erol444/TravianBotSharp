using System;
using System.Collections.Generic;
using System.IO;

namespace TbsBrowser.ChromeExtension
{
    public static class FingerPrintDefender
    {
        public static string[] GetPath()
        {
            var extenstionDir = Path.Combine(AppContext.BaseDirectory, "Assets", "ChromeExtension");
            var list = new List<string>
            {
                Path.Combine(extenstionDir, "canvas-fingerprint-defend.crx"),
                Path.Combine(extenstionDir, "audiocontext-fingerprint.crx"),
                Path.Combine(extenstionDir, "font-fingerprint-defender.crx"),
                Path.Combine(extenstionDir, "webgl-fingerprint-defende.crx"),
                Path.Combine(extenstionDir, "spoof-timezone.crx"),
            };

            return list.ToArray();
        }
    }
}