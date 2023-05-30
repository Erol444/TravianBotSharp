using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;

namespace MainCore.Helper.Implementations.Base
{
    public class InvalidPageHelper : IInvalidPageHelper
    {
        private readonly IChromeManager _chromeManager;
        private readonly ISystemPageParser _systemPageParser;
        private readonly INavigationBarParser _navigationBarParser;

        public InvalidPageHelper(IChromeManager chromeManager, ISystemPageParser systemPageParser, INavigationBarParser navigationBarParser)
        {
            _chromeManager = chromeManager;
            _systemPageParser = systemPageParser;
            _navigationBarParser = navigationBarParser;
        }

        public Result CheckPage(int accountId)
        {
            if (IsLoginPage(accountId))
            {
                return Result.Fail(new Login());
            }
            if (IsInvalidPage(accountId))
            {
                return Result.Fail(Stop.Announcement);
            }
            return Result.Ok();
        }

        private bool IsInvalidPage(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var resourceButton = _navigationBarParser.GetBuildingButton(doc);
            if (resourceButton is null) return false;
            return true;
        }

        private bool IsLoginPage(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var username = _systemPageParser.GetUsernameNode(doc);
            var password = _systemPageParser.GetPasswordNode(doc);
            var button = _systemPageParser.GetLoginButton(doc);
            return username is not null && password is not null && button is not null;
        }
    }
}