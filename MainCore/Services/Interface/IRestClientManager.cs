using MainCore.Models.Runtime;
using RestSharp;

namespace MainCore.Services.Interface
{
    public interface IRestClientManager
    {
        public RestClient Get(ProxyInfo proxyInfo);

        public void Shutdown();
    }
}