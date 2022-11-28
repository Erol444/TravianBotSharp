using ServerModuleCore;

namespace TravianOfficialCore
{
    public class UrlValidator : IUrlValidator
    {
        public bool IsFarmList(string url)
        {
            if (url.Contains("id=39")) return true;
            return false;
        }
    }
}