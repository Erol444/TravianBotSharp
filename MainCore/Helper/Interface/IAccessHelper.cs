using RestSharp;

namespace MainCore.Helper.Interface
{
    public interface IAccessHelper
    {
        bool IsValid(RestClient client);
    }
}