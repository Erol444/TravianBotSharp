using System;
using System.Net.Http;
using TbsCore.Helpers;

namespace TravBotSharp.Files.Helpers
{
    public static class Utils
    {
        public static readonly HttpClient HttpClient = new HttpClient();
    }
}
