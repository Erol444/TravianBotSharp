using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Database;
using TbsCore.Models.AccModels;

namespace TbsCore.Helpers
{
    public static class ProxyHelper
    {
        public static async Task TestProxies(Account acc)
        {
            var accesses = acc.Access.AllAccess;
            var tasks = new List<Task<bool>>();

            foreach (var a in accesses)
            {
                var client = RestClientDatabase.Instance.GetRestClientIP(a);
                tasks.Add(TestProxy(client, a.Proxy));
            }
            await Task.WhenAll(tasks);
            for (int i = 0; i < accesses.Count; i++)
            {
                accesses[i].Ok = tasks[i].Result;
            }
        }

        public static async Task<bool> TestProxy(RestClient client, string proxyIp)
        {
            var request = new RestRequest
            {
                Method = Method.Get,
            };
            try
            {
                var response = await client.ExecuteAsync(request);
                return response.Content.Equals(proxyIp);
            }
            catch
            {
                return false;
            }
        }
    }
}