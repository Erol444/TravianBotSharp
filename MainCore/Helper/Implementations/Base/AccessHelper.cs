using MainCore.Helper.Interface;
using RestSharp;
using System;

namespace MainCore.Helper.Implementations.Base
{
    public sealed class AccessHelper : IAccessHelper
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
            catch (Exception e)
            {
                _ = e;
                return false;
            }
        }
    }
}