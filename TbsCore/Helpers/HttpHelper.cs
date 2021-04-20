﻿using System;
using System.Net;
using HtmlAgilityPack;
using RestSharp;
using TbsCore.Models.Access;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Files.Helpers
{
    public static class HttpHelper
    {
        public static CookieContainer GetCookies(Account acc)
        {
            var cookies = acc.Wb.GetCookies();

            var cookieContainer = new CookieContainer();
            var cookiesStr = "";
            foreach (var cookie in cookies) cookiesStr += $"{cookie.Key}={cookie.Value},";
            cookiesStr = cookiesStr.Remove(cookiesStr.Length - 1);

            cookieContainer.SetCookies(new Uri(acc.AccInfo.ServerUrl), cookiesStr);
            return cookieContainer;
        }

        public static string SendPostReq(Account acc, RestRequest req)
        {
            acc.Wb.RestClient.CookieContainer = GetCookies(acc);

            var response = acc.Wb.RestClient.Execute(req);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("SendGetReq failed!\n" + response.Content);

            return response.Content;
        }

        public static HtmlDocument SendGetReq(Account acc, string url)
        {
            acc.Wb.RestClient.CookieContainer = GetCookies(acc);

            var req = new RestRequest
            {
                Resource = url,
                Method = Method.GET
            };

            var response = acc.Wb.RestClient.Execute(req);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("SendGetReq failed!" + response.StatusCode);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response.Content);

            return htmlDoc;
        }

        public static RestClient InitRestClient(Access access, string baseUrl)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(baseUrl),
                Timeout = 5000
            };
            client.UserAgent = access.UserAgent;

            // Set proxy
            if (!string.IsNullOrEmpty(access.Proxy))
            {
                if (!string.IsNullOrEmpty(access.ProxyUsername)) // Proxy auth
                {
                    ICredentials credentials = new NetworkCredential(access.ProxyUsername, access.ProxyPassword);
                    client.Proxy = new WebProxy($"{access.Proxy}:{access.ProxyPort}", false, null, credentials);
                }
                else // Without proxy auth
                {
                    client.Proxy = new WebProxy(access.Proxy, access.ProxyPort);
                }
            }

            client.AddDefaultHeader("Accept", "*/*");

            return client;
        }
    }
}