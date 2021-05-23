using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Models.Access;

namespace TbsCore.Helpers
{
    public static class ProxyHelper
    {
        public static async Task TestProxies(List<Access> access)
        {
            var tasks = new List<Task<bool>>();
            var restClient = new RestClient("https://api.ipify.org/");

            foreach (var a in access)
            {
                HttpHelper.InitRestClient(a, restClient);
                tasks.Add(ProxyHelper.TestProxy(restClient, a.Proxy));
            }
            await Task.WhenAll(tasks);
            for (int i = 0; i < access.Count; i++)
            {
                access[i].Ok = tasks[i].Result;
            }
        }

        /*public static async Task<bool> TestProxy(Account acc) =>
            await TestProxy(acc.Wb.RestClient, acc.Access.GetCurrentAccess().Proxy);
        */

        public static async Task<bool> TestProxy(RestClient client, string proxyIp)
        {
            var response = await client.ExecuteAsync(new RestRequest
            {
                Resource = "",
                Method = Method.GET,
            });

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response.Content);

            var ip = doc.DocumentNode.InnerText;
            return ip == proxyIp;
        }
    }
}