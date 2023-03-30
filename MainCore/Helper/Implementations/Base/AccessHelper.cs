using MainCore.Helper.Interface;
using RestSharp;

namespace MainCore.Helper.Implementations.Base
{
    public class AccessHelper : IAccessHelper
    {
        public bool IsValid(RestClient client)
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