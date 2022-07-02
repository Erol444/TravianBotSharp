using RestSharp;
using System;

namespace MainCore.Services
{
    public interface IRestClientManager : IDisposable
    {
        public RestClient Get(int id);
    }
}