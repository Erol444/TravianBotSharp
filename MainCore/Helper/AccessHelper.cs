using RestSharp;

namespace MainCore.Helper
{
    public static class AccessHelper
    {
        public static bool CheckAccess(RestClient client, string ip)
        {
            var request = new RestRequest
            {
                Method = Method.Get,
            };
            try
            {
                var response = client.Execute(request);
                return response.Content.Equals(ip);
            }
            catch
            {
                return false;
            }
        }
    }
}