using MainCore.Models.Database;
using RestSharp;

namespace MainCore.Helper.Interface
{
    public interface IAccessHelper
    {
        Access GetNextAccess(int accountId);
        bool IsLastAccess(int accountId, Access access);
        bool IsValid(RestClient client);
    }
}