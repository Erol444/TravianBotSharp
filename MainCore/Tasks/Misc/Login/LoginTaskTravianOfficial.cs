using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Misc.Login
{
    public class LoginTaskTravianOfficial : LoginTask
    {
        public LoginTaskTravianOfficial(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                AcceptCookie,
                Login,
                AddTask,
            };

            foreach (var command in commands)
            {
                _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                var result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            return Result.Ok();
        }

        private Result AcceptCookie()
        {
            var html = _chromeBrowser.GetHtml();

            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var driver = _chromeBrowser.GetChrome();
                var acceptCookie = driver.FindElements(By.ClassName("cmpboxbtnyes"));
                var result = _navigateHelper.Click(AccountId, acceptCookie[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result Login()
        {
            var html = _chromeBrowser.GetHtml();

            var usernameNode = GetUsernameNode(html);

            var passwordNode = GetPasswordNode(html);

            var buttonNode = GetLoginButton(html);
            if (buttonNode is null)
            {
                _logManager.Information(AccountId, "Account is already logged in. Skip login task");
                return Result.Ok();
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
            var account = context.Accounts.Find(AccountId);
            var access = context.Accesses.Where(x => x.AccountId == AccountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();
            var chrome = _chromeBrowser.GetChrome();

            var usernameElement = chrome.FindElements(By.XPath(usernameNode.XPath));
            if (usernameElement.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find username box"));
            }
            var passwordElement = chrome.FindElements(By.XPath(passwordNode.XPath));
            if (passwordElement.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find password box"));
            }
            var buttonElements = chrome.FindElements(By.XPath(buttonNode.XPath));
            if (buttonElements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find login button"));
            }

            usernameElement[0].SendKeys(Keys.Home);
            usernameElement[0].SendKeys(Keys.Shift + Keys.End);
            usernameElement[0].SendKeys(account.Username);

            passwordElement[0].SendKeys(Keys.Home);
            passwordElement[0].SendKeys(Keys.Shift + Keys.End);
            passwordElement[0].SendKeys(access.Password);

            buttonElements[0].Click();
            return Result.Ok();
        }

        #region parser

        private static HtmlNode GetUsernameNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("name"));
        }

        private static HtmlNode GetPasswordNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("password"));
        }

        private static HtmlNode GetLoginButton(HtmlDocument doc)
        {
            var trNode = doc.DocumentNode.Descendants("tr").FirstOrDefault(x => x.HasClass("loginButtonRow"));
            if (trNode == null) return null;
            return trNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
        }

        #endregion parser
    }
}