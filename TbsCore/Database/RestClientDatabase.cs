using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using TbsCore.Models.Access;
using TbsCore.Models.AccModels;

namespace TbsCore.Database
{
    internal class RestClientDatabase
    {
        private readonly RestClient client = new RestClient();
        private readonly Dictionary<string, RestClient> database = new Dictionary<string, RestClient>();

        private RestClientDatabase()
        {
        }

        private static RestClientDatabase instance = null;

        public static RestClientDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RestClientDatabase();
                }
                return instance;
            }
        }

        public RestClient GetLocalClient => client;

        /// <summary>
        /// Get RestClient for travian request only, only call this in function
        /// run after Wb ran
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="access">Access</param>
        /// <returns>RestClient is set, has baseUrl is acc's serverUrl </returns>
        public RestClient GetRestClientTravian(Account acc, Access access)
        {
            if (database.TryGetValue(access.UseragentHash, out RestClient value))
            {
                return value;
            }

            var clientOptions = new RestClientOptions
            {
                Timeout = 5000,
                BaseUrl = new Uri(acc.AccInfo.ServerUrl),
                UserAgent = access.Useragent,
                Proxy = GetWebProxy(access),
            };

            var client = new RestClient(clientOptions);

            var cookies = acc.Wb.GetCookies();
            foreach (var cookie in cookies)
            {
                client.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
            }

            database.Add(access.UseragentHash, client);
            return client;
        }

        /// <summary>
        /// Get RestClient for IP check request
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="access">Access</param>
        /// <returns>RestClient is set, has baseUrl is "https://api.ipify.org" </returns>
        public RestClient GetRestClientIP(Account acc, Access access)
        {
            if (database.TryGetValue($"{access.UseragentHash}_", out RestClient value))
            {
                return value;
            }

            var clientOptions = new RestClientOptions
            {
                Timeout = 5000,
                BaseUrl = new Uri("https://api.ipify.org"),
                UserAgent = access.Useragent,
                Proxy = GetWebProxy(access),
            };

            var client = new RestClient(clientOptions);

            database.Add($"{access.UseragentHash}_", client);
            return client;
        }

        private WebProxy GetWebProxy(Access access)
        {
            if (!string.IsNullOrEmpty(access.Proxy))

            {
                if (!string.IsNullOrEmpty(access.ProxyUsername)) // Proxy auth

                {
                    ICredentials credentials = new NetworkCredential(access.ProxyUsername, access.ProxyPassword);

                    return new WebProxy($"{access.Proxy}:{access.ProxyPort}", false, null, credentials);
                }
                else // Without proxy auth
                {
                    return new WebProxy(access.Proxy, access.ProxyPort);
                }
            }
            return null;
        }
    }
}