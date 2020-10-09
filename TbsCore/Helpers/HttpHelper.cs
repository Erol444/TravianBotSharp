using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Helpers
{
    public static class HttpHelper
    {
        public static string GetAjaxToken(Account acc)
        {
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    IJavaScriptExecutor js = acc.Wb.Driver as IJavaScriptExecutor;
                    return js.ExecuteScript("return ajaxToken;") as string;
                default:
                    return "";
            }
        }

        public static (CookieContainer, string) GetCookies(Account acc)
        {
            var cookies = acc.Wb.GetCookes();

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

        internal static async Task<string> SendPostReq(Account acc, FormUrlEncodedContent content, string url)
        {
            (CookieContainer container, string phpsessid) = HttpHelper.GetCookies(acc);

            using (var handler = new HttpClientHandler() { CookieContainer = container })
            using (var client = new HttpClient(handler) { BaseAddress = new System.Uri(acc.AccInfo.ServerUrl) })
            {
                var headers = client.DefaultRequestHeaders;
                headers.Add("Cookie", "PHPSESSID=" + phpsessid + ";");
                headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));

                HttpResponseMessage result = null;
                var success = false;
                do
                {
                    result = await client.PostAsync(url, content);
                    if (result.StatusCode == HttpStatusCode.OK) success = true;
                }
                while (!success);

                return await result.Content.ReadAsStringAsync();
            }
        }
        public static async Task<HtmlAgilityPack.HtmlDocument> SendGetReq(Account acc, string url)
        {
            (CookieContainer container, string phpsessid) = HttpHelper.GetCookies(acc);

            using (var handler = new HttpClientHandler() { CookieContainer = container })
            using (var client = new HttpClient(handler) { BaseAddress = new System.Uri(acc.AccInfo.ServerUrl) })
            {
                var headers = client.DefaultRequestHeaders;
                headers.Add("Cookie", "PHPSESSID=" + phpsessid + ";");
                headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));

                HttpResponseMessage result = null;
                var success = false;
                do
                {
                    result = await client.GetAsync(url);
                    if (result.StatusCode == HttpStatusCode.OK) success = true;
                }
                while (!success);

                var html = await result.Content.ReadAsStringAsync();

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                return htmlDoc;
            }
        }
    }
}
