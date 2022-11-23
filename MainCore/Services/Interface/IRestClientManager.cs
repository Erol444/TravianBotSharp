using MainCore.Models.Runtime;
using RestSharp;
using System;

namespace MainCore.Services.Interface
{
    public interface IRestClientManager : IDisposable
    {
        public RestClient Get(ProxyInfo proxyInfo);
    }
}