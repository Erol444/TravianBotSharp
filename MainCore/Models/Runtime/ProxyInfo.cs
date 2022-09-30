using MainCore.Models.Database;

namespace MainCore.Models.Runtime
{
    public class ProxyInfo
    {
        public ProxyInfo(string proxyHost = null, int proxyPort = -1, string proxyUsername = null, string proxyPassword = null)
        {
            ProxyHost = proxyHost;
            ProxyPort = proxyPort;
            ProxyUsername = proxyUsername;
            ProxyPassword = proxyPassword;
        }

        public ProxyInfo(Access access)
        {
            ProxyHost = access.ProxyHost;
            ProxyPort = access.ProxyPort;
            ProxyUsername = access.ProxyUsername;
            ProxyPassword = access.ProxyPassword;
        }

        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
    }
}