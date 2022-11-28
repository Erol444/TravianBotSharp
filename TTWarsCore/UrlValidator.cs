using ServerModuleCore;

namespace TTWarsCore
{
    public class UrlValidator : IUrlValidator
    {
        public bool IsFarmList(string url)
        {
            if (url.Contains("id=99")) return true;
            return false;
        }
    }
}