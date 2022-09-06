using MainCore.Models.Runtime;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MainCore.Services
{
    public sealed class RestClientManager : IRestClientManager
    {
        public RestClientManager(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;

            var clientOptions = new RestClientOptions
            {
                MaxTimeout = 10000,
            };

            var client = new RestClient(clientOptions);
            var key = GetKey(new ProxyInfo());
            _database.Add(key, client);
        }

        public RestClient Get(ProxyInfo proxyInfo)
        {
            var key = GetKey(proxyInfo);
            if (_database.TryGetValue(key, out RestClient client))
            {
                return client;
            }

            IWebProxy proxy = null;
            if (!string.IsNullOrEmpty(proxyInfo.ProxyHost))
            {
                if (!string.IsNullOrEmpty(proxyInfo.ProxyUsername)) // Proxy auth

                {
                    ICredentials credentials = new NetworkCredential(proxyInfo.ProxyUsername, proxyInfo.ProxyPassword);

                    proxy = new WebProxy($"{proxyInfo.ProxyHost}:{proxyInfo.ProxyPort}", false, null, credentials);
                }
                else // Without proxy auth
                {
                    proxy = new WebProxy(proxyInfo.ProxyHost, proxyInfo.ProxyPort);
                }
            }

            var clientOptions = new RestClientOptions
            {
                MaxTimeout = 10000,
                BaseUrl = new Uri("https://api.ipify.org"),
                Proxy = proxy,
            };

            client = new RestClient(clientOptions);
            _database.Add(key, client);
            return client;
        }

        public void Dispose()
        {
            foreach (var item in _database.Values)
            {
                item.Dispose();
            }
        }

        private static int GetKey(ProxyInfo proxyInfo)
        {
            var key = string.IsNullOrWhiteSpace(proxyInfo.ProxyHost) ? "default" : proxyInfo.ProxyHost;

            using var sha256Hasher = SHA256.Create();
            var hashed = sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(key));
            var ivalue = BitConverter.ToInt32(hashed, 0);
            return ivalue;
        }

        private readonly Dictionary<int, RestClient> _database = new();
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}