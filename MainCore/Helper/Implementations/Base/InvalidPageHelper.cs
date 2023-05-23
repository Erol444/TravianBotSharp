using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public class InvalidPageHelper : IInvalidPageHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IChromeManager _chromeManager;
        protected readonly ISystemPageParser _systemPageParser;
        protected readonly INavigationBarParser _navigationBarParser;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public InvalidPageHelper(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser, INavigationBarParser navigationBarParser)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _systemPageParser = systemPageParser;
            _navigationBarParser = navigationBarParser;
        }

        public void Load(int accountId, CancellationToken cancellationToken)
        {
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
        }

        public Result CheckPage()
        {
            if (IsLoginPage())
            {
                _result = Result.Fail(new Login());
                return _result;
            }
            if (IsInvalidPage())
            {
                _result = Result.Fail(Stop.Announcement);
                return _result;
            }
            return Result.Ok();
        }

        private bool IsInvalidPage()
        {
            var doc = _chromeBrowser.GetHtml();
            var resourceButton = _navigationBarParser.GetBuildingButton(doc);
            if (resourceButton is null) return false;
            return true;
        }

        private bool IsLoginPage()
        {
            var doc = _chromeBrowser.GetHtml();
            var username = _systemPageParser.GetUsernameNode(doc);
            var password = _systemPageParser.GetPasswordNode(doc);
            var button = _systemPageParser.GetLoginButton(doc);
            return username is not null && password is not null && button is not null;
        }
    }
}