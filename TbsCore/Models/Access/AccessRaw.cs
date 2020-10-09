using System;

namespace TravBotSharp.Files.Models.AccModels
{
    public class AccessRaw
    {
        /// <summary>
        /// Password for this access
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Proxy ip for this access
        /// </summary>
        public string Proxy { get; set; }
        /// <summary>
        /// Proxy port for this access
        /// </summary>
        public int ProxyPort { get; set; }
        /// <summary>
        /// Username for proxy basic authentication
        /// </summary>
        public string ProxyUsername { get; set; }
        /// <summary>
        /// Password for proxy basic authentication
        /// </summary>
        public string ProxyPassword { get; set; }
    }
}