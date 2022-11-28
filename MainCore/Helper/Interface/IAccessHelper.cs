using RestSharp;

namespace MainCore.Helper.Interface
{
    public interface IAccessHelper
    {
        bool CheckAccess(RestClient client);
    }
}