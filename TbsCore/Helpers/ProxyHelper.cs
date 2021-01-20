using HtmlAgilityPack;
using RestSharp;
using SeleniumProxyAuth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Models.Access;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Helpers
{
    public static class ProxyHelper
    {
        // TODO: dispose!
        public static SeleniumProxyServer proxyServer = new SeleniumProxyServer();
        public static async Task TestProxies(List<Access> access)
        {
            List<Task> tasks = new List<Task>(access.Count);
            access.ForEach(a =>
            {
                tasks.Add(Task.Run(() =>
                {
                    var restClient = HttpHelper.InitRestClient(a, "https://api.ipify.org/");
                    a.Ok = ProxyHelper.TestProxy(restClient, a.Proxy);
                }));
            });
            await Task.WhenAll(tasks);
        }

        public static bool TestProxy(Account acc) =>
            TestProxy(acc.Wb.RestClient, acc.Access.GetCurrentAccess().Proxy);

        public static bool TestProxy(RestClient client, string proxyIp)
        {
            var baseUrl = client.BaseUrl;

            client.BaseUrl = new Uri("https://api.ipify.org/");

            var response = client.Execute(new RestRequest
            {
                Resource = "",
                Method = Method.GET,
            });

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response.Content);

            var ip = doc.DocumentNode.InnerText;

            client.BaseUrl = baseUrl;
            return ip == proxyIp;
        }
    }
}