using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using TbsCore.Database;
using TbsCore.Models.AccModels;

namespace TbsCore.Helpers
{
    public static class HttpHelper
    {
        public static async Task<string> SendPostReqAsync(Account acc, RestRequest req)
        {
            var client = RestClientDatabase.Instance.GetRestClientTravian(acc, acc.Access.GetCurrentAccess());
            var response = await client.ExecuteAsync(req);
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("SendPostRequest failed!\n" + response.Content);

            return response.Content;
        }

        public static async Task<HtmlAgilityPack.HtmlDocument> SendGetReqAsync(Account acc, string url)
        {
            var client = RestClientDatabase.Instance.GetRestClientTravian(acc, acc.Access.GetCurrentAccess());

            var req = new RestRequest
            {
                Resource = url,
                Method = Method.Get,
            };

            var response = await client.ExecuteAsync(req);
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("SendGetReq failed!" + response.StatusCode);

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(response.Content);

            return htmlDoc;
        }
    }
}