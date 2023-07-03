using MainCore.Models.Database;
using RestSharp;

namespace MainCore.Helper.Interface
{
    public interface IAccessHelper
    {
        (Access, bool) GetNextAccess(int accountId);

        bool IsValid(RestClient client);
    }
}