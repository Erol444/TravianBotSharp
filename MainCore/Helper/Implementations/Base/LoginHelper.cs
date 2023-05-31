using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public class LoginHelper : ILoginHelper
    {
        protected readonly IChromeManager _chromeManager;

        protected readonly IGeneralHelper _generalHelper;
        protected readonly IUpdateHelper _updateHelper;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;

        protected readonly ISystemPageParser _systemPageParser;

        public LoginHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser, IUpdateHelper updateHelper)
        {
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
            _systemPageParser = systemPageParser;
            _updateHelper = updateHelper;
        }

        public Result Execute(int accountId)
        {
            var result = AcceptCookie(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = Login(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            _updateHelper.Update(accountId);
            return Result.Ok();
        }

        private Result AcceptCookie(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var result = _generalHelper.Click(accountId, By.ClassName("cmpboxbtnyes"), false);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result Login(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

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
            var account = context.Accounts.Find(accountId);
            var access = context.Accesses.Where(x => x.AccountId == accountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();

            var result = _generalHelper.Input(accountId, By.XPath(usernameNode.XPath), account.Username);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Input(accountId, By.XPath(passwordNode.XPath), access.Password);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Click(accountId, By.XPath(buttonNode.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}