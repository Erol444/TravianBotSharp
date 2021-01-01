using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TbsCore.Models.Access;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Files.Helpers
{
    public static class HttpHelper
    {
        public static (CookieContainer, string) GetCookies(Account acc)
        {
            var cookies = acc.Wb.GetCookies();

            cookies.TryGetValue("PHPSESSID", out var phpsessid);

            var cookieContainer = new CookieContainer();
            var cookiesStr = "";
            foreach (var cookie in cookies)
            {
                cookiesStr += $"{cookie.Key}={cookie.Value},";
            }
            cookiesStr = cookiesStr.Remove(cookiesStr.Length - 1);

            cookieContainer.SetCookies(new System.Uri(acc.AccInfo.ServerUrl), cookiesStr);
            return (cookieContainer, phpsessid);
        }

        public static string SendPostReq(Account acc, RestRequest req)
        {
            (CookieContainer container, string phpsessid) = HttpHelper.GetCookies(acc);

            acc.Wb.RestClient.CookieContainer = container;
            
            req.AddHeader("Cookie", "PHPSESSID=" + phpsessid + ";");

            var response = acc.Wb.RestClient.Execute(req);
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("SendGetReq failed!\n" + response.Content);

            return response.Content;
        }

        public static HtmlAgilityPack.HtmlDocument SendGetReq(Account acc, string url)
        {
            (CookieContainer container, string phpsessid) = HttpHelper.GetCookies(acc);

            acc.Wb.RestClient.CookieContainer = container;

            var req = new RestRequest
            {
                Resource = url,
                Method = Method.GET,
            };
            req.AddHeader("Cookie", "PHPSESSID=" + phpsessid + ";");

            var response = acc.Wb.RestClient.Execute(req);
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("SendGetReq failed!" + response.StatusCode);

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
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
