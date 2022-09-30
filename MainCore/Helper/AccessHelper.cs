using RestSharp;

namespace MainCore.Helper
{
    public static class AccessHelper
    {
        public static bool CheckAccess(RestClient client)
        {
            var request = new RestRequest
            {
                Method = Method.Get,
            };
            try
            {
                var response = client.Execute(request);
                return !string.IsNullOrWhiteSpace(response.Content);
            }
            catch
            {
                return false;
            }
        }
    }
}