using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public class LoginHelper : ILoginHelper
    {
        protected readonly IChromeManager _chromeManager;

        protected readonly IGeneralHelper _generalHelper;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;

        protected readonly ISystemPageParser _systemPageParser;

        protected Result _result;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public LoginHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser)
        {
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
            _systemPageParser = systemPageParser;
        }

        public void Load(int accountId, CancellationToken cancellationToken)
        {
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);

            _generalHelper.Load(-1, accountId, cancellationToken);
        }

        public Result Execute()
        {
            _result = AcceptCookie();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = Login();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.ToDorf1();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Result AcceptCookie()
        {
            var html = _chromeBrowser.GetHtml();

            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var result = _generalHelper.Click(By.ClassName("cmpboxbtnyes"), false);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result Login()
        {
            var html = _chromeBrowser.GetHtml();

            var usernameNode = _systemPageParser.GetUsernameNode(html);

            var passwordNode = _systemPageParser.GetPasswordNode(html);

            var buttonNode = _systemPageParser.GetLoginButton(html);
            if (buttonNode is null)
            {
                return Result.Fail(new Skip("Login button not found."));
            }

            if (usernameNode is null)
            {
                return Result.Fail(new Retry("Cannot find username box"));
            }

            if (passwordNode is null)
            {
                return Result.Fail(new Retry("Cannot find password box"));
            }

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(_accountId);
            var access = context.Accesses.Where(x => x.AccountId == _accountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();

            _result = _generalHelper.Input(By.XPath(usernameNode.XPath), account.Username);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Input(By.XPath(passwordNode.XPath), access.Password);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Click(By.XPath(buttonNode.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}