using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

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
            _database.Add(-1, client);
        }

        public RestClient Get(int id)
        {
            if (_database.TryGetValue(id, out RestClient client))
            {
                return client;
            }

            using var context = _contextFactory.CreateDbContext();

            var access = context.Accesses.Find(id);

            IWebProxy proxy = null;
            if (!string.IsNullOrEmpty(access.ProxyHost))
            {
                if (!string.IsNullOrEmpty(access.ProxyUsername)) // Proxy auth

                {
                    ICredentials credentials = new NetworkCredential(access.ProxyUsername, access.ProxyPassword);

                    proxy = new WebProxy($"{access.ProxyHost}:{access.ProxyPort}", false, null, credentials);
                }
                else // Without proxy auth
                {
                    proxy = new WebProxy(access.ProxyHost, access.ProxyPort);
                }
            }

            var clientOptions = new RestClientOptions
            {
                MaxTimeout = 10000,
                BaseUrl = new Uri("https://api.ipify.org"),
                Proxy = proxy,
            };

            client = new RestClient(clientOptions);
            _database.Add(id, client);
            return client;
        }

        public void Dispose()
        {
            foreach (var item in _database.Values)
            {
                item.Dispose();
            }
        }

        private readonly Dictionary<int, RestClient> _database = new();
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}